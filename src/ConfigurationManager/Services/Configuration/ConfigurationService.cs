﻿namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Configuration
{
    using ForexMiner.Heimdallr.Utilities.Data.Configuration.Configuration;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ConfigurationService : IConfigurationService
    {
        public Task<IEnumerable<ConfigurationDTO>> GetConfigurationsByNamespace(string configurationNamespace)
        {
            throw new System.NotImplementedException();
        }

        public Task<ConfigurationDTO> GetConfiguration(string configurationNamespace, string configurationName)
        {
            throw new System.NotImplementedException();
        }
    }
}
