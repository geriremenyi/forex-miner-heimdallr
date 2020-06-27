namespace ForexMiner.Heimdallr.ConfigurationManager.Controllers.V1
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/users/{userId}/configurations/namespaces/{configurationNamespace}/name/{configurationName}")]
    public class UserConfigurationController : ControllerBase
    {
    }
}
