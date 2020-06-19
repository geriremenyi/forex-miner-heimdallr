namespace ForexMiner.Heimdallr.UserManager.Services
{
    using ForexMiner.Heimdallr.DTO.User;
    using System;
    using System.Collections.Generic;

    public interface IUserService
    {
        IEnumerable<UserDTO> GetAllUsers();

        UserDTO GetUserById(Guid userId);

        UserDTO CreateUser(RegistrationDTO registration);

        UserDTO UpdateUser(Guid userId, UserUpdateDTO userUpdate);

        void DeleteUser(Guid userId);
    }
}
