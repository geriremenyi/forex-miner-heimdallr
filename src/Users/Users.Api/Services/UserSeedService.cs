//----------------------------------------------------------------------------------------
// <copyright file="UserSeedService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Users.Api.Services
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using ForexMiner.Heimdallr.Common.Data.Database.Models.User;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Entity seed service implementation for user seeding
    /// </summary>
    public class UserSeedService : IEntitySeedService
    {
        /// <summary>
        /// Configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// User service
        /// </summary>
        private readonly IUserService _userSevice;

        /// <summary>
        /// User entity seed service constructor
        /// 
        /// Setups the database context
        /// </summary>
        /// <param name="dbContext"></param>
        public UserSeedService(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userSevice = userService;
        }

        public void Seed()
        {
            var superAdminEmail = _configuration["SuperAdmin-Email"];
            var user = _userSevice.GetUserByEmail(superAdminEmail);
            if (user == null)
            {
                _userSevice.Register(new Registration()
                {
                    Email = superAdminEmail,
                    FirstName = "Gergely",
                    LastName = "Reményi",
                    Password = _configuration["SuperAdmin-Password"]
                }, Role.Admin );
            }
        }
    }
}
