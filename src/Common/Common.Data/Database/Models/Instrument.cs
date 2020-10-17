//----------------------------------------------------------------------------------------
// <copyright file="Instrument.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Database.Models
{
    using GeriRemenyi.Oanda.V20.Client.Model;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Representation of a known instrument by the application
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Instrument
    {
        /// <summary>
        /// Name of the instrument
        /// </summary>
        [Key]
        public InstrumentName Name { get; set; }

        /// <summary>
        /// Is the instrument tradable by the application users
        /// </summary>
        [Required]
        public bool IsTradeable { get; set; }
    }
}
