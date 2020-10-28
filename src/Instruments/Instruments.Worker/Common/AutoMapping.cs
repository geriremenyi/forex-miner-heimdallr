namespace ForexMiner.Heimdallr.Instruments.Worker.Common
{
    using AutoMapper;
    using Oanda = GeriRemenyi.Oanda.V20.Client.Model;
    using Contracts = Heimdallr.Common.Data.Contracts.Instrument;
    using AutoMapper.Extensions.EnumMapping;
    using System;

    public class AutoMapping : Profile
    {

        public AutoMapping()
        {
            CreateMap<Oanda.InstrumentName, Contracts.InstrumentName>().ConvertUsingEnumMapping(options => options.MapByName()).ReverseMap();
            CreateMap<Oanda.CandlestickGranularity, Contracts.Granularity>().ConvertUsingEnumMapping(options => options.MapByName()).ReverseMap();
            CreateMap<Oanda.Candlestick, Contracts.Candle>()
                .ForMember(
                    dest => dest.Time,
                    opt => opt.MapFrom(src => DateTime.Parse(src.Time).ToUniversalTime())
                ).ReverseMap();
            CreateMap<Oanda.CandlestickData, Contracts.Candlestick>()
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
        }

    }
}
