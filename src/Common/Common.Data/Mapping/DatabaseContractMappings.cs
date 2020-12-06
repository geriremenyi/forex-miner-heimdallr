//----------------------------------------------------------------------------------------
// <copyright file="DatabaseContractMappings.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Mapping
{
    using AutoMapper;
    using Database = Database.Models;
    using Contracts = Contracts;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Database types to Contract types
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DatabaseContractMappings : Profile
    {
        /// <summary>
        /// Database to contract mappings constructor.
        /// Defines the known mappings.
        /// </summary>
        public DatabaseContractMappings()
        {
            // User
            CreateMap<Database.User.User, Contracts.User.User>().ReverseMap();
            CreateMap<Database.User.User, Contracts.User.Registration>().ReverseMap();
            CreateMap<Database.User.User, Contracts.User.LoggedInUser>().ReverseMap();

            // Instrument
            CreateMap<Database.Instrument.Instrument, Contracts.Instrument.InstrumentCreation>().ReverseMap();
            CreateMap<Database.Instrument.Instrument, Contracts.Instrument.Instrument>().ReverseMap();
            CreateMap<Database.Instrument.InstrumentGranularity, Contracts.Instrument.InstrumentGranularityCreation >().ReverseMap();
            CreateMap<Database.Instrument.InstrumentGranularity, Contracts.Instrument.InstrumentGranularity>().ReverseMap();

            // Connections
            CreateMap<Database.Connection.Connection, Contracts.Connection.Connection>().ReverseMap();
            CreateMap<Database.Connection.Connection, Contracts.Connection.ConnectionCreation>().ReverseMap();

            // Trades
            CreateMap<Database.Trade.TradeSignal, Contracts.Trade.TradeSignal>().ReverseMap();
        }
    }
}
