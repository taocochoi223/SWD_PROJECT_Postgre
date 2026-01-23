using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD.API.Dtos;
using SWD.BLL.Interfaces;
using SWD.DAL.Models;

namespace SWD.API.Controllers
{
    [Route("api/hubs")]
    [ApiController]
    [Authorize]
    public class HubController : ControllerBase
    {
        private readonly IHubService _hubService;

        public HubController( IHubService hubService)
        {
            _hubService = hubService;
        }
        /// <summary>
        /// Get all hubs
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllHubsAsync()
        {
            var hubs = await _hubService.GetAllHubsAsync();
            var hubDtos = hubs.Select(h => new HubDto
            {
                HubId = h.HubId,
                Name = h.Name,
                MacAddress = h.MacAddress,
                IsOnline = h.IsOnline,
                LastHandshake = h.LastHandshake,
                SiteId = h.SiteId,
                SiteName = h.Site?.Name ?? "Unassigned",
                SensorCount = h.Sensors?.Count ?? 0
            }).ToList();
            return Ok(hubDtos);
        }

        /// <summary>
        /// Get hub by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHubByIdAsync(int id){
            var hub = await _hubService.GetHubByIdAsync(id);
            if (hub == null)
                return NotFound(new { message = "Hub not found" });
            var hubDto = new HubDto
            {
                HubId = hub.HubId,
                Name = hub.Name,
                MacAddress = hub.MacAddress,
                IsOnline = hub.IsOnline,
                LastHandshake = hub.LastHandshake,
                SiteId = hub.SiteId,
                SiteName = hub.Site?.Name ?? "Unassigned",
                SensorCount = hub.Sensors?.Count ?? 0
            };
            return Ok(hubDto);
        }

        /// <summary>
        /// Create hub
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,ADMIN,Manager,MANAGER")]
        public async Task<IActionResult> CreateHubAsync([FromBody] RegisterHubDto request)
        {
            var ex = await _hubService.GetHubByMacAsync(request.MacAddress);
            if (ex != null)
            {
                return BadRequest(new { message = "Hub with the same MAC address already exists." });
            }

            var hub = new Hub
            {
                SiteId = request.SiteId,
                Name = request.Name,
                MacAddress = request.MacAddress,
                IsOnline = false 
            };

            await _hubService.CreateHubAsync(hub);

            // Thay CreatedAtAction bằng Ok
            return Ok(new
            {
                message = "Hub created successfully",
                hub = new HubDto
                {
                    HubId = hub.HubId,
                    Name = hub.Name,
                    MacAddress = hub.MacAddress,
                    IsOnline = hub.IsOnline,
                    SiteId = hub.SiteId,
                    SensorCount = 0
                }
            });
        }

        /// <summary>
        /// Update hub
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,ADMIN,Manager,MANAGER")]
        public async Task<IActionResult> UpdateHubAsync(int id, [FromBody] UpdateHubDto request)
        {
            var existingHub = await _hubService.GetHubByIdAsync(id);
            if (existingHub == null)
                return NotFound(new { message = "Hub not found" });

            if (request.SiteId.HasValue)
                existingHub.SiteId = request.SiteId.Value;

            if (!string.IsNullOrEmpty(request.Name))
                existingHub.Name = request.Name;

            await _hubService.UpdateHubAsync(existingHub);
            return Ok(new 
            { 
                message = "Hub updated successfully", hubId = existingHub.HubId 
            });
        }

        /// <summary>
        /// Delete Hub - Delete a hub
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,ADMIN,Manager,MANAGER")]
        public async Task<IActionResult> DeleteHubAsync(int id)
        {
            var existingHub = await _hubService.GetHubByIdAsync(id);
            if (existingHub == null)
                return NotFound(new { message = "Hub not found" });

            await _hubService.DeleteHubAsync(id);

            return Ok(new 
            { 
                message = "Hub deleted successfully", hubId = id 
            });
        }
    }
}
