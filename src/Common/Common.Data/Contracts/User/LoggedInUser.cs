//----------------------------------------------------------------------------------------
// <copyright file="LoggedInUser.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.User
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IdentityModel.Tokens.Jwt;
    using System.Runtime.Serialization;
    using System.Security.Claims;
    using System.Text;

    /// <summary>
    /// Representation of a user which is logged in
    /// 
    /// Basically a database user without credential info but with the authentication token
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class LoggedInUser : User
    {
        /// <summary>
        /// JWT token to autehnticate with
        /// </summary>
        [DataMember(Name = "token")]
        public string Token { get; set; }

        /// <summary>
        /// Generate new JWT token for the user
        /// </summary>
        /// <param name="loggedInUser">The logged in user to generate the token to</param>
        /// <param name="issuerSigningKey">The key to sign the token with</param>
        public void AddNewJwtToken(string issuerSigningKey)
        {
            // Generate claims
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Role, Role.ToString())
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
            Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
