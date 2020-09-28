
namespace ForexMiner.Heimdallr.Configurations.Api.Services.Configuration
{
    using ForexMiner.Heimdallr.Common.Data.Configuration.Configuration;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IConfigurationService
    {
        public Task<IEnumerable<ConfigurationDTO>> GetConfigurationsByNamespace(string configurationNamespace);
        public Task<ConfigurationDTO> GetConfiguration(string configurationNamespace, string configurationName);
    }
}
