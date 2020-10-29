//----------------------------------------------------------------------------------------
// <copyright file="OandaContractMappings.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Mapping
{
    using System;
    using AutoMapper;
    using AutoMapper.Extensions.EnumMapping;
    using Oanda = GeriRemenyi.Oanda.V20.Client.Model;
    using Contracts = Contracts;
    using OandaSdk = GeriRemenyi.Oanda.V20.Sdk;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Oanda types to Contract types
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class OandaContractMappings : Profile
    {
        /// <summary>
        /// Oanda to contract mappings constructor.
        /// Defines the known mappings.
        /// </summary>
        public OandaContractMappings()
        {
            // Instrument, granularity enums
            CreateMap<Oanda.InstrumentName, Contracts.Instrument.InstrumentName>().ConvertUsingEnumMapping(options => options.MapByName()).ReverseMap();
            CreateMap<Oanda.CandlestickGranularity, Contracts.Instrument.Granularity>().ConvertUsingEnumMapping(options => options.MapByName()).ReverseMap();

            // Candles, candlesticks TODO: align naming in client/SDK and here
            CreateMap<Oanda.Candlestick, Contracts.Instrument.Candle>().ForMember(
                dest => dest.Time,
                opt => opt.MapFrom(src => DateTime.Parse(src.Time).ToUniversalTime())
            ).ReverseMap();
            CreateMap<Oanda.CandlestickData, Contracts.Instrument.Candlestick>().ForMember(
                dest => dest.Open,
                opt => opt.MapFrom(src => src.O)
            ).ForMember(
                dest => dest.High,
                opt => opt.MapFrom(src => src.H)
            ).ForMember(
                dest => dest.Low,
                opt => opt.MapFrom(src => src.L)
            ).ForMember(
                dest => dest.Close,
                opt => opt.MapFrom(src => src.C)
            ).ReverseMap();

            // Trades
            CreateMap<Oanda.Trade, Contracts.Trade.Trade>().ReverseMap();
            CreateMap<OandaSdk.Trade.TradeDirection, Contracts.Trade.TradeDirection>().ConvertUsingEnumMapping(options => options.MapByName()).ReverseMap(); ;
        }
    }
}
