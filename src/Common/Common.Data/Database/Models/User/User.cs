//----------------------------------------------------------------------------------------
// <copyright file="User.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Models.User
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models.Connection;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

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
    }
}
