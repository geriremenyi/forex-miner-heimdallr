
namespace ForexMiner.Heimdallr.ConfigurationManager.Services.Configuration
{
    using ForexMiner.Heimdallr.Utilities.Data.Configuration.Configuration;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IConfigurationService
    {
        public Task<IEnumerable<ConfigurationDTO>> GetConfigurationsByNamespace(string configurationNamespace);
        public Task<ConfigurationDTO> GetConfiguration(string configurationNamespace, string configurationName);
    }
}
