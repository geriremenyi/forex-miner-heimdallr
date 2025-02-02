﻿//----------------------------------------------------------------------------------------
// <copyright file="UserLogin.cs" company="geriremenyi.com">
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
    /// User login data transfer object representing a login request
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class UserLogin
    {
        /// <summary>
        /// Email to login with
        /// </summary>
        [DataMember(Name = "email")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Password to login with
        /// </summary>
        [DataMember(Name = "password")]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}
