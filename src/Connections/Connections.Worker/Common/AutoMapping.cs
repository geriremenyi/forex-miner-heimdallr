namespace ForexMiner.Heimdallr.Connections.Worker.Common
{
    using AutoMapper;
    using Oanda = GeriRemenyi.Oanda.V20.Client.Model;
    using Contracts = Heimdallr.Common.Data.Contracts;
    using Database = Heimdallr.Common.Data.Database.Models;
    using AutoMapper.Extensions.EnumMapping;
    using System;

    public class AutoMapping : Profile
    {

        public AutoMapping()
        {
            CreateMap<Oanda.InstrumentName, Contracts.Instrument.InstrumentName>().ConvertUsingEnumMapping(options => options.MapByName()).ReverseMap();
            CreateMap<Oanda.CandlestickGranularity, Contracts.Instrument.Granularity>().ConvertUsingEnumMapping(options => options.MapByName()).ReverseMap();
            CreateMap<Oanda.Candlestick, Contracts.Instrument.Candle>()
                .ForMember(
                    dest => dest.Time,
                    opt => opt.MapFrom(src => DateTime.Parse(src.Time).ToUniversalTime())
                ).ReverseMap();
            CreateMap<Oanda.CandlestickData, Contracts.Instrument.Candlestick>()
                .ForMember(
                    dest => dest.Open,
                    opt => opt.MapFrom(src => src.O)
                )
                .ForMember(
                    dest => dest.High,
                    opt => opt.MapFrom(src => src.H)
                )
                .ForMember(
                    dest => dest.Low,
                    opt => opt.MapFrom(src => src.L)
                )
                .ForMember(
                    dest => dest.Close,
                    opt => opt.MapFrom(src => src.C)
                ).ReverseMap();
            CreateMap<Contracts.Trade.TradeSignal, Database.Trade.TradeSignal>().ReverseMap();
        }

    }
}
