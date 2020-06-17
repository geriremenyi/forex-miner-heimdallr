﻿namespace ForexMiner.Heimdallr.UserManager.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using ForexMiner.Heimdallr.Contracts.User;

    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<UserContract> Get()
        {            
            return new List<UserContract>()
            {
                new UserContract()
                {
                    UserId = new Guid(),
                    EmailAddress = "user1@forex-miner.com",
                    FirstName = "User1",
                    LastName = "ForexMiner"
                },
                new UserContract()
                {
                    UserId = new Guid(),
                    EmailAddress = "user2@forex-miner.com",
                    FirstName = "User2",
                    LastName = "ForexMiner"
                }
            };
        }

        [HttpGet("{userId}")]
        public UserContract Get(Guid userId)
        {
            return new UserContract()
            {
                UserId = userId,
                EmailAddress = $"{userId}@forex-miner.com",
                FirstName = $"{userId}",
                LastName = "ForexMiner"
            };
        }

        [HttpPost]
        public UserContract Post([FromBody] RegistrationContract newUser)
        {
            return new UserContract()
            {
                UserId = new Guid(),
                EmailAddress = newUser.EmailAddress,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName
            };
        }

        [HttpPut("{userId}")]
        public UserContract Put(Guid userId, [FromBody] RegistrationContract modifiedUser)
        {
            return new UserContract()
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
