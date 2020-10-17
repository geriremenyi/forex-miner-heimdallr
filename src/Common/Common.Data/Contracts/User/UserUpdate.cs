//----------------------------------------------------------------------------------------
// <copyright file="UserUpdate.cs" company="geriremenyi.com">
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
    /// Represents a user update data transfer object
    /// </summary>
    [ExcludeFromCodeCoverage]
    [DataContract]
    public class UserUpdate
    {
        /// <summary>
        /// Email of the user to update to
        /// </summary>
        [DataMember]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// First name of the user to update to
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the user to upate to
        /// </summary>
        [DataMember]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// Password of the user to update to
        /// </summary>
        [DataMember]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
