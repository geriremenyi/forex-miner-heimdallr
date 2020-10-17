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
    using Database = Heimdallr.Common.Data.Database.Models;
    using Contracts = Heimdallr.Common.Data.Contracts.User;

    public class UserService : IUserService
    {
        private readonly ForexMinerHeimdallrDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(ForexMinerHeimdallrDbContext dbContext, IMapper mapper, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public IEnumerable<Contracts.User> GetAllUsers()
        {
            return _mapper.Map<IEnumerable<Database.User>, IEnumerable<Contracts.User>>(_dbContext.Users);
        }

        public Contracts.User GetUserById(Guid userId)
        {
            return _mapper.Map<Database.User, Contracts.User>(GetUserFromDbById(userId));
        }

        public Contracts.User CreateUser(Contracts.Registration registration)
        {
            // Check if email address is not taken already
            var userWithSameEmail = GetUserFromDbByEmail(registration.Email);
            if (userWithSameEmail != null)
            {
                throw new ProblemDetailsException(HttpStatusCode.BadRequest, $"User with the email address {registration.Email} already registered.");
            }

            // Map registration to user
            var user = _mapper.Map<Contracts.Registration, Database.User>(registration);

            // Secure password with salt
            user.UpdatePassword(registration.Password);

            // Save user to DB
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return _mapper.Map<Database.User, Contracts.User>(user);
        }

        public Contracts.User UpdateUser(Guid userId, Contracts.UserUpdate userUpdate)
        {
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

            return _mapper.Map<Database.User, Contracts.User>(user);
        }

        public Contracts.LoggedInUser Authenticate(Contracts.Authentication authentication)
        {
            var user = GetUserFromDbByEmail(authentication.Email);
            if (user == null || user.IsPasswordCorrect(authentication.Password))
            {
                throw new ProblemDetailsException(HttpStatusCode.NotFound, "Invalid email address or password");
            }

            var authResponse = _mapper.Map<Database.User, Contracts.LoggedInUser>(user);
            authResponse.AddNewJwtToken(_configuration["JWT:IssuerSigningKey"]);

            return authResponse;
        }

        public void DeleteUser(Guid userId)
        {
            _dbContext.Remove(GetUserFromDbById(userId));
            _dbContext.SaveChanges();
        }

        private Database.User GetUserFromDbById(Guid userId)
        {
            var user = _dbContext.Users.SingleOrDefault(user => user.Id == userId);
            if (user == null)
            {
                throw new ProblemDetailsException(HttpStatusCode.NotFound, $"User with id {userId} not found.");
            }

            return user;
        }

        private Database.User GetUserFromDbByEmail(string email)
        {
            return _dbContext.Users.SingleOrDefault(user => user.Email == email);
        }
    }
}
