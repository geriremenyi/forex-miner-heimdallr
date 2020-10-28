//----------------------------------------------------------------------------------------
// <copyright file="UserExtension.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Users.Api.Common
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models.User;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// User extensions
    /// </summary>
    public static class UserExtension
    {
        /// <summary>
        /// Update the password via regenerating salt and hashing it with the plain text password
        /// </summary>
        /// <param name="user">User to update the password for</param>
        /// <param name="plainPassword">New plan text password</param>
        public static void UpdatePassword(this User user, string plainPassword)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            var password = HashPassword(plainPassword, salt);

            user.PasswordSalt = salt.ToString();
            user.PasswordHash = password;
        }

        /// <summary>
        /// Check the hashed password agains the plain text password via hashing it with the salt
        /// </summary>
        /// <param name="user">The user to check the password for</param>
        /// <param name="plainPassword">The plain text password</param>
        /// <returns></returns>
        public static bool IsPasswordCorrect(this User user, string plainPassword)
        {
            var hashedPassword = HashPassword(plainPassword, Encoding.ASCII.GetBytes(user.PasswordSalt));

            return hashedPassword.Equals(user.PasswordHash);
        }

        /// <summary>
        /// Hash the plain text password with the given salt
        /// </summary>
        /// <param name="plainPassword"></param>
        /// <param name="salt"></param>
        /// <returns>The hashed password</returns>
        private static string HashPassword(string plainPassword, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: plainPassword,
               salt: salt,
               prf: KeyDerivationPrf.HMACSHA1,
               iterationCount: 10000,
               numBytesRequested: 256 / 8)
            );
        }
    }
}
