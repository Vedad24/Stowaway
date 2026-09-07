using Stowaway.Application.Modules.Dashboard.Queries.GetGlobalStats;

namespace Stowaway.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DashboardController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<GetGlobalStatsQueryDto> GetGlobalStats(CancellationToken ct)
        {
            var result = await sender.Send(new GetGlobalStatsQuery(), ct);
            return result;
        }
    }
}
