//----------------------------------------------------------------------------------------
// <copyright file="InstrumentName.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Contracts.Instrument
{

    using System.Runtime.Serialization;

    /// <summary>
    /// Available instrument names in the application
    /// Currently only the seven major currency pairs are supported
    /// </summary>
    public enum InstrumentName
    {
        /// <summary>
        /// Enum EUR_USD for value: EUR_USD
        /// </summary>
        [EnumMember(Value = "EUR_USD")]
        EUR_USD,

        /// <summary>
        /// Enum GBP_USD for value: GBP_USD
        /// </summary>
        [EnumMember(Value = "GBP_USD")]
        GBP_USD,

        /// <summary>
        /// Enum USD_CHF for value: USD_CHF
        /// </summary>
        [EnumMember(Value = "USD_CHF")]
        USD_CHF,


        /// <summary>
        /// Enum AUD_USD for value: AUD_USD
        /// </summary>
        [EnumMember(Value = "AUD_USD")]
        AUD_USD,

        /// <summary>
        /// Enum USD_CAD for value: USD_CAD
        /// </summary>
        [EnumMember(Value = "USD_CAD")]
        USD_CAD,

        /// <summary>
        /// Enum NZD_USD for value: NZD_USD
        /// </summary>
        [EnumMember(Value = "NZD_USD")]
        NZD_USD,
    }
}
