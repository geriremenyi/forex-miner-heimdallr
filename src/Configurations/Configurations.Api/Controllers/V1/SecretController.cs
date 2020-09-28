namespace ForexMiner.Heimdallr.Configurations.Api.Controllers.V1
{
    using ForexMiner.Heimdallr.Common.Data.Configuration.Secret;
    using ForexMiner.Heimdallr.Configurations.Api.Services.Secret;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/secrets/namespaces/{secretNamespace}/names/{secretName}")]
    public class SecretController : ControllerBase
    {
        private readonly ISecretService _secretService;

        public SecretController(ISecretService secretService)
        {
            _secretService = secretService;
        }

        [HttpGet]
        public async Task<SecretDTO> GetConfiguration(string secretNamespace, string secretName)
        {
            return await _secretService.GetSecret(secretNamespace, secretName);
        }
    }
}
