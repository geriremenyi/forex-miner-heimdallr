//----------------------------------------------------------------------------------------
// <copyright file="LoggedInUser.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.User
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

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
    }
}
