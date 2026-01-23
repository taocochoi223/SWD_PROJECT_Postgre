using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWD.API.Dtos;
using SWD.BLL.Interfaces;
using SWD.DAL.Models;

namespace SWD.API.Controllers
{
    [Route("api/site")]
    [ApiController]
    [Authorize]
    public class SiteController : ControllerBase
    {
        private readonly ISiteService _siteService;

        public SiteController(ISiteService siteService)
        {
            _siteService = siteService;
        }

        /// <summary>
        /// Get All Sites
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSitesAsync([FromQuery] string? search = null)
        {
            var sites = await _siteService.GetAllSitesAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                sites = sites.Where(s =>
                    s.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    s.SiteId.ToString().Contains(search)
                ).ToList();
            }

            var siteDtos = sites.Select(s => new SiteDto
            {
                SiteId = s.SiteId,
                OrgId = s.OrgId,
                OrgName = s.Org?.Name ?? "Unknown",
                Name = s.Name,
                Address = s.Address,
                GeoLocation = s.GeoLocation,
                HubCount = s.Hubs?.Count ?? 0
            }).ToList();

            return Ok(siteDtos);
        }

        /// <summary>
        /// Create Site - Create a new site
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> CreateSiteAsync([FromBody] CreateSiteDto request)
        {
            var site = new Site
            {
                OrgId = request.OrgId,
                Name = request.Name,
                Address = request.Address,
                GeoLocation = request.GeoLocation
            };

            await _siteService.CreateSiteAsync(site);

            return Ok(new
            {
                Message = "Site created successfully",
                Data = new SiteDto
                {
                    SiteId = site.SiteId,
                    OrgId = site.OrgId,
                    Name = site.Name,
                    Address = site.Address,
                    GeoLocation = site.GeoLocation,
                    HubCount = 0
                }
            });
        }

        /// <summary>
        /// Get Site By ID - Helper endpoint for CreatedAtAction
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSiteByIdAsync(int id)
        {
            var site = await _siteService.GetSiteByIdAsync(id);
            if (site == null)
                return NotFound(new { message = "Site not found" });

            var siteDto = new SiteDto
            {
                SiteId = site.SiteId,
                OrgId = site.OrgId,
                OrgName = site.Org?.Name ?? "Unknown",
                Name = site.Name,
                Address = site.Address,
                GeoLocation = site.GeoLocation,
                HubCount = site.Hubs?.Count ?? 0
            };

            return Ok(siteDto);
        }

        /// <summary>
        /// Update Site - Update an existing site
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> UpdateSiteAsync(int id, [FromBody] CreateSiteDto request)
        {
            var existingSite = await _siteService.GetSiteByIdAsync(id);
            if (existingSite == null)
                return NotFound(new { message = "Site not found" });

            existingSite.OrgId = request.OrgId;
            existingSite.Name = request.Name;
            existingSite.Address = request.Address;
            existingSite.GeoLocation = request.GeoLocation;

            await _siteService.UpdateSiteAsync(existingSite);

            return Ok(new { message = "Site updated successfully", siteId = existingSite.SiteId });
        }

        /// <summary>
        /// Delete Site - Delete a site
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> DeleteSiteAsync(int id)
        {
            var existingSite = await _siteService.GetSiteByIdAsync(id);
            if (existingSite == null)
                return NotFound(new { message = "Site not found" });

            await _siteService.DeleteSiteAsync(id);

            return Ok(new { message = "Site deleted successfully", siteId = id });
        }
    }
}
