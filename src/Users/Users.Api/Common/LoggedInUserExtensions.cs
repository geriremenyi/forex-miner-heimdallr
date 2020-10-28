//----------------------------------------------------------------------------------------
// <copyright file="LoggedInUserExtensions" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Users.Api.Common
{
    using ForexMiner.Heimdallr.Common.Data.Contracts.User;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;

    /// <summary>
    /// Extension for the LoggedInUser class
    /// </summary>
    public static class LoggedInUserExtensions
    {
        /// <summary>
        /// Generate new JWT token for the user
        /// </summary>
        /// <param name="loggedInUser">The logged in user to generate the token to</param>
        /// <param name="issuerSigningKey">The key to sign the token with</param>
        public static void AddNewJwtToken(this LoggedInUser loggedInUser, string issuerSigningKey)
        {
            // Generate claims
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, loggedInUser.Id.ToString()),
                new Claim(ClaimTypes.Email, loggedInUser.Email),
                new Claim(ClaimTypes.Role, loggedInUser.Role.ToString())
            };

            // Construct token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(issuerSigningKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            // Extend logged in user with the token created
            loggedInUser.Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
