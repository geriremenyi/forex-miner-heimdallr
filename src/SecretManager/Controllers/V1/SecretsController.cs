namespace ForexMiner.Heimdallr.SecretManager.Controllers
{
    using ForexMiner.Heimdallr.Data.Secret;
    using ForexMiner.Heimdallr.SecretManager.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/secrets/namespaces/{secretNamespace}/name/{secretName}")]
    public class SecretsController : ControllerBase
    {
        private readonly ISecretService _secretService;

        public SecretsController(ISecretService secretService)
        {
            _secretService = secretService;
        }

        public SecretDTO GetSecret(string secretNamespace, string secretName)
        {
            return _secretService.GetSecret(secretNamespace, secretName);
        }

    }
}
