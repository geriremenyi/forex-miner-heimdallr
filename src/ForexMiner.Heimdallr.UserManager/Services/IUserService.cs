namespace ForexMiner.Heimdallr.UserManager.Services
{
    using ForexMiner.Heimdallr.Contracts.User;
    using System.Collections.Generic;

    public interface IUserService
    {
        IEnumerable<UserDTO> GetAllUsers();
    }
}
