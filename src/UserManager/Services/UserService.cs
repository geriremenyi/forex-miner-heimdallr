namespace ForexMiner.Heimdallr.UserManager.Services
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Utilities.Data.Constants;
    using ForexMiner.Heimdallr.Utilities.Data.Exceptions;
    using ForexMiner.Heimdallr.Utilities.Data.User;
    using ForexMiner.Heimdallr.UserManager.Database;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Caching.Library.Service;
    using ForexMiner.Heimdallr.UserManager.Common;
    using Microsoft.Extensions.Configuration;

    public class UserService : IUserService
    {
        private readonly UserManagerDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(UserManagerDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
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

        public AuthenticationResponseDTO Authenticate(AuthenticationDTO authentication)
        {
            var user = GetUserFromDbByEmail(authentication.EmailAddress);
            if (user == null || user.IsPasswordCorrect(authentication.Password))
            {
                throw new ProblemDetailsException(HttpStatusCode.NotFound, "Invalid email address or password");
            }

            var authResponse = _mapper.Map<User, AuthenticationResponseDTO>(user);
            authResponse.AddNewJwtToken(_configuration["JWT:IssuerSigningKey"]);

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
