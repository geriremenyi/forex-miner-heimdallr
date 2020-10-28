//----------------------------------------------------------------------------------------
// <copyright file="Registration.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.User
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Representation of a registration
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class Registration
    {
        /// <summary>
        /// Email address to register with
        /// </summary>
        [DataMember(Name = "email")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// First name ti register with
        /// </summary>
        [DataMember(Name = "firstName")]
        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name to register with
        /// </summary>
        [DataMember(Name = "lastName")]
        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Password to register with
        /// </summary>
        [DataMember(Name = "password")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}
