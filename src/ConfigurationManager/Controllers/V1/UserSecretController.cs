namespace ForexMiner.Heimdallr.ConfigurationManager.Controllers.V1
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/users/{userId}/secrets/namespaces/{secretNamespace}/name/{secretName}")]
    public class UserSecretController : ControllerBase
    {
    }
}
