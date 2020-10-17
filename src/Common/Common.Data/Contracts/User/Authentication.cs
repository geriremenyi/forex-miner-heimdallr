//----------------------------------------------------------------------------------------
// <copyright file="Autentication.cs" company="geriremenyi.com">
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
    /// Authentication data transfer object representing a login request
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class Authentication
    {
        /// <summary>
        /// Email to login with
        /// </summary>
        [DataMember]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Password to login with
        /// </summary>
        [DataMember]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
