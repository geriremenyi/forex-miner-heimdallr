//----------------------------------------------------------------------------------------
// <copyright file="AutoMapping.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Connections.Api.Common
{
    using AutoMapper;
    using Contracts = Heimdallr.Common.Data.Contracts;
    using Database = Heimdallr.Common.Data.Database.Models;
    using Oanda = GeriRemenyi.Oanda.V20.Client.Model;

    /// <summary>
    /// Auto mapping for instruments
    /// </summary>
    public class AutoMapping : Profile
    {

        /// <summary>
        /// Auto mapping for instruments constructor
        /// Creates all possible instrument mappings
        /// </summary>
        public AutoMapping()
        {
            CreateMap<Contracts.Connection.Connection, Database.Connection.Connection>().ReverseMap();
            CreateMap<Contracts.Connection.ConnectionCreation, Database.Connection.Connection>().ReverseMap();
            CreateMap<Contracts.Connection.ConnectionCreation, Contracts.Connection.ConnectionTest> ().ReverseMap();
            CreateMap<Contracts.Trade.TradeSignal, Database.Trade.TradeSignal>().ReverseMap();
            CreateMap<Oanda.InstrumentName, Contracts.Instrument.InstrumentName>().ReverseMap();
            CreateMap<Oanda.Trade, Contracts.Trade.Trade>().ReverseMap();
        }

    }
}
