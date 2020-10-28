//----------------------------------------------------------------------------------------
// <copyright file="UserService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Users.Api.Services
{
    using AutoMapper;
    using ForexMiner.Heimdallr.Common.Data.Exceptions;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using ForexMiner.Heimdallr.Users.Api.Common;
    using Microsoft.Extensions.Configuration;
    using Database = Heimdallr.Common.Data.Database.Models.User;
    using Contracts = Heimdallr.Common.Data.Contracts.User;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// User service implementation
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Database context
        /// </summary>
        private readonly ForexMinerHeimdallrDbContext _dbContext;

        /// <summary>
        /// Object auto mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Application configurations
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initialize user service
        /// 
        /// Setups the database context, the object auto mapper and the configuration
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        /// <param name="configuration"></param>
        public UserService(ForexMinerHeimdallrDbContext dbContext, IMapper mapper, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        /// <summary>
        /// Login with user details
        /// </summary>
        /// <param name="userLogin">User login details</param>
        /// <returns>The logged in user with it's token</returns>
        public Contracts.LoggedInUser Login(Contracts.UserLogin userLogin)
        {
            // Check user if is present in the DB
            var user = GetUserFromDbByEmail(userLogin.Email);
            if (user == null || user.IsPasswordCorrect(userLogin.Password))
            {
                throw new ProblemDetailsException(HttpStatusCode.NotFound, "Invalid email address or password");
            }

            // Generate and add JWT token to the logged in user object
            var loggedInUser = _mapper.Map<Database.User, Contracts.LoggedInUser>(user);
            loggedInUser.AddNewJwtToken(_configuration["JWT-IssuerSigningKey"]);

            return loggedInUser;
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="registration">The registration details, the user to create</param>
        /// <returns>The registered user</returns>
        public Contracts.User Register(Contracts.Registration registration, Database.Role? role = null)
        {
            // Check if email address is not taken already
            var userWithSameEmail = GetUserFromDbByEmail(registration.Email);
            if (userWithSameEmail != null)
            {
                throw new ProblemDetailsException(HttpStatusCode.BadRequest, $"User with the email address {registration.Email} already registered.");
            }

            // Map registration to user
            var user = _mapper.Map<Contracts.Registration, Database.User>(registration);

            // Add role
            user.Role = role ?? Database.Role.Trader;

            // Secure password with salt
            user.UpdatePassword(registration.Password);

            // Save user to DB
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return _mapper.Map<Database.User, Contracts.User>(user);
        }

        /// <summary>
        /// Get user details by id
        /// </summary>
        /// <param name="userId">Get details of a user</param>
        /// <returns>The user</returns>
        public Contracts.User GetUserById(Guid userId)
        {
            return _mapper.Map<Database.User, Contracts.User>(GetUserFromDbById(userId));
        }

        /// <summary>
        /// Get user details by email
        /// </summary>
        /// <param name="userId">Get details of a user</param>
        /// <returns>The user</returns>
        public Contracts.User GetUserByEmail(string email)
        {
            return _mapper.Map<Database.User, Contracts.User>(GetUserFromDbByEmail(email));
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="userId">Id of the user to update</param>
        /// <param name="userUpdate">User update object</param>
        /// <returns>The updated user</returns>
        public Contracts.User UpdateUserById(Guid userId, Contracts.UserUpdate userUpdate)
        {
            // Get the user
            var user = GetUserFromDbById(userId);

            // Update simple types
            user.Email = userUpdate.Email ?? user.Email;
            user.FirstName = userUpdate.FirstName ?? user.FirstName;
            user.LastName = userUpdate.LastName ?? user.LastName;

            // Update the password
            if (userUpdate.Password != null)
            {
                user.UpdatePassword(userUpdate.Password);
            }

            // Save it in the DB
            _dbContext.SaveChanges();

            return _mapper.Map<Database.User, Contracts.User>(user);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="userId">The id of the user to delete</param>
        public void DeleteUserById(Guid userId)
        {
            _dbContext.Remove(GetUserFromDbById(userId));
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>A list of users</returns>
        public IEnumerable<Contracts.User> GetAllUsers()
        {
            return _mapper.Map<IEnumerable<Database.User>, IEnumerable<Contracts.User>>(_dbContext.Users);
        }

        /// <summary>
        /// Get the user from the DB by it's id
        /// </summary>
        /// <param name="userId">Id of the user to get</param>
        /// <returns>The user</returns>
        private Database.User GetUserFromDbById(Guid userId)
        {
            // Check that user exists in the database
            var user = _dbContext.Users
                .Include(user => user.Connections)
                .SingleOrDefault(user => user.Id == userId);

            if (user == null)
            {
                throw new ProblemDetailsException(HttpStatusCode.NotFound, $"User with id {userId} not found.");
            }

            return user;
        }

        /// <summary>
        /// Get the user from the DB by it's email address
        /// </summary>
        /// <param name="email">Email address of the user to get</param>
        /// <returns>The user</returns>
        private Database.User GetUserFromDbByEmail(string email)
        {
            return _dbContext.Users
                .Include(user => user.Connections)
                .SingleOrDefault(user => user.Email == email);
        }
    }
}
