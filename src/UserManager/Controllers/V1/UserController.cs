namespace ForexMiner.Heimdallr.UserManager.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using ForexMiner.Heimdallr.Utilities.Data.User;
    using ForexMiner.Heimdallr.UserManager.Services;
    using Microsoft.AspNetCore.Authorization;
    using System.Threading.Tasks;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IEnumerable<UserDTO> GetUsers()
        {
            return _userService.GetAllUsers();
        }

        [HttpGet("{userId}")]
        public UserDTO GetUser(Guid userId)
        {
            return _userService.GetUserById(userId);
        }

        [AllowAnonymous]
        [HttpPost]
        public UserDTO Register([FromBody] RegistrationDTO registration)
        {
            return _userService.CreateUser(registration);
        }

        [HttpPatch("{userId}")]
        public UserDTO UpdateUser(Guid userId, [FromBody] UserUpdateDTO userUpdate)
        {
            return _userService.UpdateUser(userId, userUpdate);
        }

        [HttpDelete("{userId}")]
        public void DeleteUser(Guid userId)
        {
            _userService.DeleteUser(userId);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public AuthenticationResponseDTO Authenticate([FromBody] AuthenticationDTO authentication)
        {
            return _userService.Authenticate(authentication);
        }
    }
}
