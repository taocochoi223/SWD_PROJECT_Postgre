using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD.BLL.Interfaces;
using SWD.API.Dtos;
using System.Linq;

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

        /// <summary>
        /// Get Hierarchy (Site -> Hub -> Sensor)
        /// </summary>
        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetHierarchyAsync()
        {
            var sites = await _siteService.GetSiteHierarchyAsync();
            
            var siteDtos = sites.Select(s => new SiteDashboardDto
            {
                SiteId = s.SiteId,
                Name = s.Name,
                Address = s.Address,
                Hubs = s.Hubs?.Select(h => new HubDashboardDto
                {
                    HubId = h.HubId,
                    Name = h.Name,
                    MacAddress = h.MacAddress,
                    IsOnline = h.IsOnline,
                    LastHandshake = h.LastHandshake,
                    Sensors = h.Sensors?.Select(se => new SensorDashboardDto
                    {
                        SensorId = se.SensorId,
                        Name = se.Name,
                        TypeName = se.Type?.TypeName ?? "Unknown",
                        Unit = se.Type?.Unit ?? "",
                        CurrentValue = (float?)se.CurrentValue,
                        LastUpdate = se.LastUpdate,
                        TotalReadings = se.Readings?.Count ?? 0
                    }).ToList() ?? new List<SensorDashboardDto>()
                }).ToList() ?? new List<HubDashboardDto>()
            }).ToList();

            return Ok(siteDtos);
        }

        /// <summary>
        /// Get Hierarchy for specific Site (Site -> Hub -> Sensor)
        /// </summary>
        [HttpGet("site/{siteId}")]
        public async Task<IActionResult> GetHierarchyBySiteIdAsync(int siteId)
        {
            var s = await _siteService.GetSiteHierarchyByIdAsync(siteId);
            if (s == null)
            {
                return NotFound(new { message = "Site not found" });
            }

            var siteDto = new SiteDashboardDto
            {
                SiteId = s.SiteId,
                Name = s.Name,
                Address = s.Address,
                Hubs = s.Hubs?.Select(h => new HubDashboardDto
                {
                    HubId = h.HubId,
                    Name = h.Name,
                    MacAddress = h.MacAddress,
                    IsOnline = h.IsOnline,
                    LastHandshake = h.LastHandshake,
                    Sensors = h.Sensors?.Select(se => new SensorDashboardDto
                    {
                        SensorId = se.SensorId,
                        Name = se.Name,
                        TypeName = se.Type?.TypeName ?? "Unknown",
                        Unit = se.Type?.Unit ?? "",
                        CurrentValue = (float?)se.CurrentValue,
                        LastUpdate = se.LastUpdate,
                        TotalReadings = se.Readings?.Count ?? 0
                    }).ToList() ?? new List<SensorDashboardDto>()
                }).ToList() ?? new List<HubDashboardDto>()
            };

            return Ok(siteDto);
        }
    }
}
