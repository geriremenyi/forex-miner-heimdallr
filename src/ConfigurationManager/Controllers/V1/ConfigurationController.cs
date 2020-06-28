namespace ForexMiner.Heimdallr.ConfigurationManager.Controllers.V1
{
    using ForexMiner.Heimdallr.ConfigurationManager.Services.Configuration;
    using ForexMiner.Heimdallr.Utilities.Data.Configuration.Configuration;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/configurations/namespaces/{configurationNamespace}")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet]
        public async Task<IEnumerable<ConfigurationDTO>> GetConfigurationsByNamespace(string configurationNamespace)
        {
            return await _configurationService.GetConfigurationsByNamespace(configurationNamespace);
        }

        [HttpGet("names/{configurationName}")]
        public async Task<ConfigurationDTO> GetConfiguration(string configurationNamespace, string configurationName)
        {
            return await _configurationService.GetConfiguration(configurationNamespace, configurationName);
        }
    }
}
