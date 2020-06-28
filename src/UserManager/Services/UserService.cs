namespace ForexMiner.Heimdallr.UserManager.Services
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Utilities.Cache.Services;
    using ForexMiner.Heimdallr.Utilities.Cache.Types;
    using ForexMiner.Heimdallr.Utilities.Data.Constants;
    using ForexMiner.Heimdallr.Utilities.Data.Exceptions;
    using ForexMiner.Heimdallr.Utilities.Data.User;
    using ForexMiner.Heimdallr.UserManager.Database;
    using ForexMiner.Heimdallr.UserManager.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly UserManagerDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public UserService(UserManagerDbContext context, IMapper mapper, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            return _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(_context.Users);
        }

        public UserDTO GetUserById(Guid userId)
        {
            return _mapper.Map<User, UserDTO>(GetUserFromDbById(userId));
        }

        public UserDTO CreateUser(RegistrationDTO registration)
        {
            // Check if email address is not taken already
            var userWithSameEmail = GetUserFromDbByEmail(registration.EmailAddress);
            if (userWithSameEmail != null)
            {
                throw new ProblemDetailsException(HttpStatusCode.BadRequest, $"User with the email address {registration.EmailAddress} already registered.");
            }

            // Map registration to user
            var user = _mapper.Map<RegistrationDTO, User>(registration);

            // Secure password with salt
            user.UpdatePassword(registration.Password);

            // Save user to DB
            _context.Users.Add(user);
            _context.SaveChanges();

            return _mapper.Map<User, UserDTO>(user);
        }

        public UserDTO UpdateUser(Guid userId, UserUpdateDTO userUpdate)
        {
            var user = GetUserFromDbById(userId);

            // Update simple types
            user.EmailAddress = userUpdate.EmailAddress ?? user.EmailAddress;
            user.FirstName = userUpdate.FirstName ?? user.FirstName;
            user.LastName = userUpdate.LastName ?? user.LastName;

            // Update the password
            if (userUpdate.Password != null)
            {
                user.UpdatePassword(userUpdate.Password);
            }

            return _mapper.Map<User, UserDTO>(user);
        }

        public async Task<AuthenticationResponseDTO> Authenticate(AuthenticationDTO authentication)
        {
            var user = GetUserFromDbByEmail(authentication.EmailAddress);
            if (user == null || user.IsPasswordCorrect(authentication.Password))
            {
                throw new ProblemDetailsException(HttpStatusCode.NotFound, "Invalid email address or password");
            }

            var authResponse = _mapper.Map<User, AuthenticationResponseDTO>(user);
            authResponse.AddNewJwtToken(await _cacheService.GetOrCreateCacheValue(CacheType.Secret, JwtConstants.Namespace, JwtConstants.EncryptionSecret, () => "aYPg2QjKQBY4Uqx8"));

            return authResponse;
        }

        public void DeleteUser(Guid userId)
        {
            _context.Remove(GetUserFromDbById(userId));
            _context.SaveChanges();
        }

        private User GetUserFromDbById(Guid userId)
        {
            var user = _context.Users.SingleOrDefault(user => user.UserId == userId);
            if (user == null)
            {
                throw new ProblemDetailsException(HttpStatusCode.NotFound, $"User with id {userId} not found.");
            }

            return user;
        }

        private User GetUserFromDbByEmail(string EmailAddress)
        {
            return _context.Users.SingleOrDefault(user => user.EmailAddress == EmailAddress);
        }
    }
}
