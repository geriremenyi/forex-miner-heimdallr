namespace ForexMiner.Heimdallr.Users.Api.Controllers.V1
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using ForexMiner.Heimdallr.Users.Api.Services;
    using Microsoft.AspNetCore.Authorization;
    using System.Linq;
    using System.Security.Claims;
    using ForexMiner.Heimdallr.Common.Data.Exceptions;
    using System.Net;

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
            // Claim validation
            var userIdClaim = User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault();
            if (userIdClaim == null)
            {
                // User somehow doesn't have a user id in the claims
                throw new ProblemDetailsException(HttpStatusCode.Unauthorized, "Unauthorized access.");
            }
            if (Guid.Parse(userIdClaim.Value) != userId)
            {
                // User is trying to reach a different user's details
                throw new ProblemDetailsException(HttpStatusCode.Forbidden, "You don't have permissions to see other user's details.");
            }

            // Get user from service if validations are passed
            return _userService.GetUserById(userId);
        }

        [HttpPatch("{userId}")]
        public User UpdateUser(Guid userId, [FromBody] UserUpdate userUpdate)
        {
            // Claim validation
            var userIdClaim = User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault();
            if (userIdClaim == null)
            {
                // User somehow doesn't have a user id in the claims
                throw new ProblemDetailsException(HttpStatusCode.Unauthorized, "Unauthorized access.");
            }
            if (Guid.Parse(userIdClaim.Value) != userId)
            {
                // User is trying to reach a different user's details
                throw new ProblemDetailsException(HttpStatusCode.Forbidden, "You don't have permissions to see other user's details.");
            }

            // Update user if validations are passed
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
            // Claim validation
            var userIdClaim = User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault();
            if (userIdClaim == null)
            {
                // User somehow doesn't have a user id in the claims
                throw new ProblemDetailsException(HttpStatusCode.Unauthorized, "Unauthorized access.");
            }
            if (Guid.Parse(userIdClaim.Value) != userId)
            {
                // User is trying to reach a different user's details
                throw new ProblemDetailsException(HttpStatusCode.Forbidden, "You don't have permissions to see other user's details.");
            }

            // Delete user if validations are passed
            _userService.DeleteUser(userId);
        }
    }
}
