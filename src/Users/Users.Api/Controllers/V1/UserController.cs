//----------------------------------------------------------------------------------------
// <copyright file="UserController.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

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
    using Role = Heimdallr.Common.Data.Database.Models.User.Role;

    /// <summary>
    /// API controller for user endpoints
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/users")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// User service
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// User endpoint controller constructor
        /// 
        /// Initializes the user service
        /// </summary>
        /// <param name="userService"></param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// User login endpoint
        /// </summary>
        /// <param name="userLogin">User login details to login with</param>
        /// <returns>The logged in user with it's token</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public LoggedInUser Login([FromBody] UserLogin userLogin)
        {
            return _userService.Login(userLogin);
        }

        /// <summary>
        /// User registration endpoint
        /// </summary>
        /// <param name="registration"></param>
        /// <returns>The registered user</returns>
        [AllowAnonymous]
        [HttpPost]
        public User Register([FromBody] Registration registration)
        {
            return _userService.Register(registration);
        }

        /// <summary>
        /// Shortcut to the users/{userId} for the logged in user
        /// </summary>
        /// <returns>The logged in user</returns>
        [HttpGet("me")]
        public User Me()
        {
            // Find out user id
            var userIdClaim = User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault();

            // Get user from service if validations are passed
            return _userService.GetUserById(Guid.Parse(userIdClaim.Value));
        }

        /// <summary>
        /// User get endpoint
        /// </summary>
        /// <param name="userId">The id of the user</param>
        /// <returns>The user</returns>
        [HttpGet("{userId}")]
        public User GetUser(Guid userId)
        {
            // Check authorization
            CheckAuthorization(userId);

            // Get user from service if validations are passed
            return _userService.GetUserById(userId);
        }

        /// <summary>
        /// Update user endpoint
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="userUpdate">The user object to update to</param>
        /// <returns>The updated user</returns>
        [HttpPatch("{userId}")]
        public User UpdateUser(Guid userId, [FromBody] UserUpdate userUpdate)
        {
            // Check authorization
            CheckAuthorization(userId);

            // Update user if validations are passed
            return _userService.UpdateUserById(userId, userUpdate);
        }

        /// <summary>
        /// Delete user endpoint
        /// </summary>
        /// <param name="userId">The id of the user</param>
        [HttpDelete("{userId}")]
        public void DeleteUser(Guid userId)
        {
            // Check authorization
            CheckAuthorization(userId);

            // Delete user if validations are passed
            _userService.DeleteUserById(userId);
        }

        /// <summary>
        /// Get all users endpoint
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IEnumerable<User> GetAllUsers()
        {
            return _userService.GetAllUsers();
        }

        /// <summary>
        /// Check authorization
        /// </summary>
        /// <param name="userId"></param>
        private void CheckAuthorization(Guid userId, bool bypassForAdmin = true)
        {
            // Claim validation
            var userIdClaim = User.Claims.Where(claim => claim.Type == ClaimTypes.Name).FirstOrDefault();
            var shouldBypass = User.Claims.Where(claim => claim.Type == ClaimTypes.Role).Any(role => role.Value == Role.Admin.ToString()) && bypassForAdmin;
            if (userIdClaim == null)
            {
                // User somehow doesn't have a user id in the claims
                throw new ProblemDetailsException(HttpStatusCode.Unauthorized, "Unauthorized access.");
            }
            if (Guid.Parse(userIdClaim.Value) != userId && !shouldBypass)
            {
                // User is trying to reach a different user's details
                throw new ProblemDetailsException(HttpStatusCode.Forbidden, "You don't have permissions to see this.");
            }
        }
    }
}
