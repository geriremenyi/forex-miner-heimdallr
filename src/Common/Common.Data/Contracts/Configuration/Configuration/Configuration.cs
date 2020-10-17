//----------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Configuration.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Configuration data transfer object
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Configuration
    {
        /// <summary>
        /// Namespace of the configuration
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Name of the configuration
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Calue of the configuration
        /// </summary>
        public string Value { get; set; }
    }
}
