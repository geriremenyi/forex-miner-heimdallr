namespace ForexMiner.Heimdallr.UserManager.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using ForexMiner.Heimdallr.Contracts.User;
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
            return new UserDTO()
            {
                UserId = userId,
                EmailAddress = $"{userId}@forex-miner.com",
                FirstName = $"{userId}",
                LastName = "ForexMiner"
            };
        }

        [HttpPost]
        public UserDTO Post([FromBody] RegistrationDTO newUser)
        {
            return new UserDTO()
            {
                UserId = new Guid(),
                EmailAddress = newUser.EmailAddress,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName
            };
        }

        [HttpPut("{userId}")]
        public UserDTO Put(Guid userId, [FromBody] RegistrationDTO modifiedUser)
        {
            return new UserDTO()
            {
                UserId = userId,
                EmailAddress = modifiedUser.EmailAddress,
                FirstName = modifiedUser.FirstName,
                LastName = modifiedUser.LastName
            };
        }

        [HttpDelete("{userId}")]
        public void Delete(int userId)
        {
        }
    }
}
