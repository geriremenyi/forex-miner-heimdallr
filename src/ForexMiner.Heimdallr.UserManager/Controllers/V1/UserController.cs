namespace ForexMiner.Heimdallr.UserManager.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using ForexMiner.Heimdallr.DTO.User;
    using ForexMiner.Heimdallr.UserManager.Services;

    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IEnumerable<UserDTO> Get()
        {
            return _userService.GetAllUsers();
        }

        [HttpGet("{userId}")]
        public UserDTO Get(Guid userId)
        {
            return _userService.GetUserById(userId);
        }

        [HttpPost]
        public UserDTO Post([FromBody] RegistrationDTO registration)
        {
            return _userService.CreateUser(registration);
        }

        [HttpPatch("{userId}")]
        public UserDTO Patch(Guid userId, [FromBody] UserUpdateDTO userUpdate)
        {
            return _userService.UpdateUser(userId, userUpdate);
        }

        [HttpDelete("{userId}")]
        public void Delete(Guid userId)
        {
            _userService.DeleteUser(userId);
        }
    }
}
