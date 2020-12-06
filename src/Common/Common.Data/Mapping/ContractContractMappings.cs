//----------------------------------------------------------------------------------------
// <copyright file="ContractContractMappings.cs" company="geriremenyi.com">
//     Author: Gergely Reményi
//     Copyright (c) geriremenyi.com. All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------

namespace ForexMiner.Heimdallr.Common.Data.Mapping
{
    using AutoMapper;
    using Contracts = Contracts;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Contract types to other contract types
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ContractContractMappings : Profile
    {
        /// <summary>
        /// Contract to contract mappings constructor.
        /// Defines the known mappings.
        /// </summary>
        public ContractContractMappings()
        {
            // Connections
            CreateMap<Contracts.Connection.ConnectionCreation, Contracts.Connection.ConnectionTest>().ReverseMap();
        }
    }
}
