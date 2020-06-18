namespace ForexMiner.Heimdallr.UserManager.Services
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Contracts.User;
    using ForexMiner.Heimdallr.UserManager.Database;
    using System.Collections.Generic;

    public class UserService : IUserService
    {
        private readonly UserManagerDbContext _context;
        private readonly IMapper _mapper;

        public UserService(UserManagerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            return _mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(_context.Users);
        }
    }
}
