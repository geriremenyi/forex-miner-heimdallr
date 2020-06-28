namespace ForexMiner.Heimdallr.ConfigurationManager.Controllers.V1
{
    using ForexMiner.Heimdallr.ConfigurationManager.Services.Secret;
    using ForexMiner.Heimdallr.Data.Secret;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/secrets/namespaces/{secretNamespace}/name/{secretName}")]
    public class SecretController : ControllerBase
    {
        private readonly ISecretService _secretService;

        public SecretController(ISecretService secretService)
        {
            _secretService = secretService;
        }

        public async Task<SecretDTO> GetSecret(string secretNamespace, string secretName)
        {
            return await _secretService.GetSecret(secretNamespace, secretName);
        }

    }
}
