namespace ForexMiner.Heimdallr.Users.Api.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using ForexMiner.Heimdallr.Users.Api.Services;
    using Microsoft.AspNetCore.Authorization;

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

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public LoggedInUser Authenticate([FromBody] Authentication authentication)
        {
            return _userService.Authenticate(authentication);
        }

        [AllowAnonymous]
        [HttpPost]
        public User Register([FromBody] Registration registration)
        {
            return _userService.CreateUser(registration);
        }

        [HttpGet("{userId}")]
        public User GetUser(Guid userId)
        {
            return _userService.GetUserById(userId);
        }

        [HttpPatch("{userId}")]
        public User UpdateUser(Guid userId, [FromBody] UserUpdate userUpdate)
        {
            return _userService.UpdateUser(userId, userUpdate);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IEnumerable<User> GetAllUsers()
        {
            return _userService.GetAllUsers();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public void DeleteUser(Guid userId)
        {
            _userService.DeleteUser(userId);
        }
    }
}
