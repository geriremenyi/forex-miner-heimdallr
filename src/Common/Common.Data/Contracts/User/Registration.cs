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
    [DataContract]
    [ExcludeFromCodeCoverage]
    public class Registration
    {
        /// <summary>
        /// Email address to register with
        /// </summary>
        [DataMember]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// First name ti register with
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name to register with
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// Password to register with
        /// </summary>
        [DataMember]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
