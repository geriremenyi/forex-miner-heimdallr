namespace ForexMiner.Heimdallr.Users.Api.Services
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using System;
    using System.Collections.Generic;

    public interface IUserService
    {
        public IEnumerable<User> GetAllUsers();

        public User GetUserById(Guid userId);

        public User CreateUser(Registration registration);

        public User UpdateUser(Guid userId, UserUpdate userUpdate);

        public void DeleteUser(Guid userId);

        public LoggedInUser Authenticate(Authentication authentication);
    }
}
