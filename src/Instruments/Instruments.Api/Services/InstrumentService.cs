//----------------------------------------------------------------------------------------
// <copyright file="InstrumentService.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Instruments.Api.Services
{
    using Contracts = Heimdallr.Common.Data.Contracts.Instrument;
    using Database = Heimdallr.Common.Data.Database.Models.Instrument;
    using ForexMiner.Heimdallr.Common.Data.Database.Context;
    using System.Linq;
    using ForexMiner.Heimdallr.Common.Data.Exceptions;
    using System.Net;
    using AutoMapper;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Instrument service implementation
    /// </summary>
    public class InstrumentService : IInstrumentService
    {
        /// <summary>
        /// Databse context
        /// </summary>
        private ForexMinerHeimdallrDbContext _dbContext;


        /// <summary>
        /// Object auto mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Instrument service implementation constructor.
        /// Sets up the database context.
        /// </summary>
        /// <param name="dbContext"></param>
        public InstrumentService(ForexMinerHeimdallrDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all instruments
        /// </summary>
        /// <returns>The list of all instruments</returns>
        public IEnumerable<Contracts.Instrument> GetAllInstruments()
        {
            return _mapper.Map<IEnumerable<Database.Instrument>, IEnumerable<Contracts.Instrument>>(_dbContext.Instruments.Include(instrument => instrument.Granularities));
        }

        /// <summary>
        /// Add a new instrument to the tradeable instruments
        /// </summary>
        /// <param name="instrument">The instrument to add to tradeable</param>
        /// <returns>The instrument added</returns>
        public Contracts.Instrument AddInstrument(Contracts.InstrumentCreation instrument)
        {
            // Check if instrument is not added already
            var instrumentInDb = GetInstumentFromDbByName(instrument.Name);
            if (instrumentInDb != null)
            {
                return _mapper.Map<Database.Instrument, Contracts.Instrument>(instrumentInDb);
            }

            // If it is not then save it
            var userToSaveInDb = _mapper.Map<Contracts.InstrumentCreation, Database.Instrument>(instrument);
            _dbContext.Instruments.Add(userToSaveInDb);
            _dbContext.SaveChanges();

            // Map back db entity to contract
            return _mapper.Map<Database.Instrument, Contracts.Instrument>(userToSaveInDb);
        }


        /// <summary>
        /// Add a new supported granularity to an existing instrument
        /// </summary>
        /// <param name="instrumentName">Name of the instrument to add the granularity to</param>
        /// <param name="granularity">Granularity to add</param>
        /// <returns>The whole instrument with the new granularity added</returns>
        public Contracts.Instrument AddGranularity(Contracts.InstrumentName instrumentName, Contracts.InstrumentGranularityCreation granularity)
        {
            // Check if instrument is in the DB
            var instrumentInDb = GetInstumentFromDbByName(instrumentName);
            if (instrumentInDb == null)
            {
                throw new ProblemDetailsException(HttpStatusCode.NotFound, $"Instrument '{instrumentName}' not found.");
            }

            // Check if granularity is in DB
            var granularityInDb = GetGranularityFromDbByName(instrumentName, granularity.Granularity);
            if (granularityInDb != null)
            {
                // If yes just return the whole instrument object
                return _mapper.Map<Database.Instrument, Contracts.Instrument>(instrumentInDb);
            }

            // If not then create it in the DB
            var granularityToAdd = _mapper.Map<Contracts.InstrumentGranularityCreation, Database.InstrumentGranularity>(granularity);
            granularityToAdd.State = Database.GranularityState.New; // With the state of new
            instrumentInDb.Granularities.Add(granularityToAdd);
            _dbContext.Update(instrumentInDb);
            _dbContext.SaveChanges();

            // Return the entity saved in the DB
            return _mapper.Map<Database.Instrument, Contracts.Instrument>(instrumentInDb);
        }

        /// <summary>
        /// Get an instrument from the DB by it's name
        /// </summary>
        /// <param name="name">Name of the instrument</param>
        /// <returns>Instrument in DB</returns>
        private Database.Instrument GetInstumentFromDbByName(Contracts.InstrumentName name)
        {
            return _dbContext.Instruments.SingleOrDefault(instrument => instrument.Name == name);
        }

        /// <summary>
        /// Get granularity from DB
        /// </summary>
        /// <param name="instrumentName">Name of the instrument</param>
        /// <param name="granularityName">Granularity</param>
        /// <returns>Granularity</returns>
        private Database.InstrumentGranularity GetGranularityFromDbByName(Contracts.InstrumentName instrumentName, Contracts.Granularity granularityName)
        {
            return _dbContext.Instruments.Include(instrument => instrument.Granularities)
                .SingleOrDefault(instrument => instrument.Name == instrumentName)?.Granularities
                .SingleOrDefault(granularity => granularity.Granularity == granularityName);
        }
    }
}
