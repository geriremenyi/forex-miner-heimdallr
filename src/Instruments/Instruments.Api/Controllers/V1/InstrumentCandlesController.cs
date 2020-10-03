namespace ForexMiner.Heimdallr.Instruments.Api.Controllers.V1
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [ApiVersion("1")]
    [Authorize]
    [Route("api/v{version:apiVersion}/instruments/{instrument}")]
    public class InstrumentCandlesController : ControllerBase
    {
    }
}
