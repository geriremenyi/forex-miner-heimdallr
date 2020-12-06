//----------------------------------------------------------------------------------------
// <copyright file="User.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Models.User
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models.Connection;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Representation of a user in the database
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class User
    {
        /// <summary>
        /// Unique identifier of the user
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Email address of the user
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// First name of the user
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the user
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// The password of the users stored as a hashed value
        /// </summary>
        [Required]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Hashing salt used for the password hash
        /// </summary>
        [Required]
        public string PasswordSalt { get; set; }


        /// <summary>
        /// Role assigned to the user
        /// </summary>
        [Required]
        public Role Role { get; set; }

        /// <summary>
        /// Connections for the user
        /// </summary>
        public ICollection<Connection> Connections { get; set; }

        /// <summary>
        /// Has the user connections added
        /// </summary>
        public bool HasConnections { get { return Connections.Count() > 0; } }

        /// <summary>
        /// Constructor of the user
        /// Initializes an empty list of connections
        /// </summary>
        public User()
        {
            Connections = new List<Connection>();
        }

        /// <summary>
        /// Update the password via regenerating salt and hashing it with the plain text password
        /// </summary>
        /// <param name="user">User to update the password for</param>
        /// <param name="plainPassword">New plan text password</param>
        public void UpdatePassword(string plainPassword)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            var password = HashPassword(plainPassword, salt);

            PasswordSalt = Convert.ToBase64String(salt);
            PasswordHash = password;
        }

        /// <summary>
        /// Check the hashed password agains the plain text password via hashing it with the salt
        /// </summary>
        /// <param name="user">The user to check the password for</param>
        /// <param name="plainPassword">The plain text password</param>
        /// <returns></returns>
        public bool IsPasswordCorrect(string plainPassword)
        {
            var hashedPassword = HashPassword(plainPassword, Convert.FromBase64String(PasswordSalt));

            return hashedPassword.Equals(PasswordHash);
        }

        /// <summary>
        /// Hash the plain text password with the given salt
        /// </summary>
        /// <param name="plainPassword"></param>
        /// <param name="salt"></param>
        /// <returns>The hashed password</returns>
        public string HashPassword(string plainPassword, byte[] salt)
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
