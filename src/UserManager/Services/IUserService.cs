﻿namespace ForexMiner.Heimdallr.UserManager.Services
{
    using ForexMiner.Heimdallr.Utilities.Data.User;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserService
    {
        public IEnumerable<UserDTO> GetAllUsers();

        public UserDTO GetUserById(Guid userId);

        public UserDTO CreateUser(RegistrationDTO registration);

        public UserDTO UpdateUser(Guid userId, UserUpdateDTO userUpdate);

        public void DeleteUser(Guid userId);

        public AuthenticationResponseDTO Authenticate(AuthenticationDTO authentication);
    }
}
