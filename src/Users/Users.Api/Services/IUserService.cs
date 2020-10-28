//----------------------------------------------------------------------------------------
// <copyright file="IUserService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Users.Api.Services
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using Role = ForexMiner.Heimdallr.Common.Data.Database.Models.User.Role;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// User service interface
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Login with user details
        /// </summary>
        /// <param name="userLogin">User login details</param>
        /// <returns>The logged in user with it's token</returns>
        public LoggedInUser Login(UserLogin userLogin);

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="registration">The registration details, the user to create</param>
        /// <returns>The registered user</returns>
        public User Register(Registration registration, Role? role = null);


        /// <summary>
        /// Get user details
        /// </summary>
        /// <param name="userId">Get details of a user</param>
        /// <returns>The user</returns>
        public User GetUserById(Guid userId);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">Get details of a user</param>
        /// <returns>The user</returns>
        public User GetUserByEmail(string email);

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="userId">Id of the user to update</param>
        /// <param name="userUpdate">User update object</param>
        /// <returns>The updated user</returns>
        public User UpdateUserById(Guid userId, UserUpdate userUpdate);

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userId">The id of the user to delete</param>
        public void DeleteUserById(Guid userId);

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>A list of users</returns>
        public IEnumerable<User> GetAllUsers();
    }
}
