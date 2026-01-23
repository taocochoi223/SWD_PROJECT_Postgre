using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD.BLL.Interfaces;

namespace SWD.API.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ISiteService _siteService;
        private readonly IHubService _hubService;
        private readonly ISensorService _sensorService;
        private readonly IAlertService _alertService;

        public DashboardController(
            ISiteService siteService,
            IHubService hubService,
            ISensorService sensorService,
            IAlertService alertService)
        {
            _siteService = siteService;
            _hubService = hubService;
            _sensorService = sensorService;
            _alertService = alertService;
        }

        /// <summary>
        /// Get Dashboard Statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStatsAsync()
        {
            var sites = await _siteService.GetAllSitesAsync();
            var hubs = await _hubService.GetAllHubsAsync();
            var sensors = await _sensorService.GetAllSensorsAsync();
            var alerts = await _alertService.GetAlertsWithFiltersAsync("Active", null);

            var stats = new
            {
                total_sites = sites.Count,
                total_hubs = hubs.Count,
                active_sensors = sensors.Count(s => s.Status == "Active"),
                pending_alerts = alerts.Count
            };

            return Ok(stats);
        }
    }
}
