//----------------------------------------------------------------------------------------
// <copyright file="User.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.User
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models.User;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    /// <summary>
    /// Representation of the user data transfer object
    /// 
    /// Basically the databse user object without credential specific information
    /// </summary>
    [DataContract]
    [ExcludeFromCodeCoverage]
    public class User
    {
        /// <summary>
        /// Id of the user
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Email address of the user
        /// </summary>
        [DataMember]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// First name of the user
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        ///  Last name of the user
        /// </summary>
        [DataMember]
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// Role assigned to the user
        /// </summary>
        [DataMember]
        [Required]
        public Role Role { get; set; }

        /// <summary>
        /// Has the user connections added already
        /// </summary>
        public bool HasConnections { get; set; }
    }
}
