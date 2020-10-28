//----------------------------------------------------------------------------------------
// <copyright file="AutoMapping.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Api.Common
{
    using AutoMapper;
    using System.Collections.Generic;
    using Contracts = Heimdallr.Common.Data.Contracts.Instrument;
    using Database = Heimdallr.Common.Data.Database.Models.Instrument;

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
            CreateMap<Contracts.InstrumentCreation, Database.Instrument>();
            CreateMap<Database.Instrument, Contracts.Instrument>();
            CreateMap<Contracts.InstrumentGranularityCreation, Database.InstrumentGranularity>();
            CreateMap<Database.InstrumentGranularity, Contracts.InstrumentGranularity>();
        }

    }
}
