using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD.API.Dtos;
using SWD.BLL.Interfaces;
using SWD.DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SWD.API.Controllers
{
    [Route("api/organizations")]
    [ApiController]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        /// <summary>
        /// Get all organizations
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> GetAllOrganizationsAsync()
        {
            var orgs = await _organizationService.GetAllOrganizationsAsync();
            var orgDtos = orgs.Select(o => new OrganizationDto
            {
                OrgId = o.OrgId,
                Name = o.Name,
                Description = o.Description,
                CreatedAt = o.CreatedAt,
                SiteCount = o.Sites?.Count ?? 0
            }).ToList();

            return Ok(orgDtos);
        }

        /// <summary>
        /// Get organization by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> GetOrganizationByIdAsync(int id)
        {
            var org = await _organizationService.GetOrganizationByIdAsync(id);
            if (org == null)
                return NotFound(new { message = "Organization not found" });

            var orgDto = new OrganizationDto
            {
                OrgId = org.OrgId,
                Name = org.Name,
                Description = org.Description,
                CreatedAt = org.CreatedAt,
                SiteCount = org.Sites?.Count ?? 0
            };

            return Ok(orgDto);
        }

        /// <summary>
        /// Create a new organization
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> CreateOrganizationAsync([FromBody] CreateOrganizationDto request)
        {
            var org = new Organization
            {
                Name = request.Name,
                Description = request.Description
            };

            var createdOrg = await _organizationService.CreateOrganizationAsync(org);

            var orgDto = new OrganizationDto
            {
                OrgId = createdOrg.OrgId,
                Name = createdOrg.Name,
                Description = createdOrg.Description,
                CreatedAt = createdOrg.CreatedAt,
                SiteCount = 0
            };

            return CreatedAtAction(nameof(GetOrganizationByIdAsync), new { id = createdOrg.OrgId }, orgDto);
        }

        /// <summary>
        /// Update an organization
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> UpdateOrganizationAsync(int id, [FromBody] UpdateOrganizationDto request)
        {
            var existingOrg = await _organizationService.GetOrganizationByIdAsync(id);
            if (existingOrg == null)
                return NotFound(new { message = "Organization not found" });

            if (!string.IsNullOrEmpty(request.Name))
                existingOrg.Name = request.Name;
            
            if (request.Description != null)
                existingOrg.Description = request.Description;

            await _organizationService.UpdateOrganizationAsync(existingOrg);

            return Ok(new { message = "Organization updated successfully", orgId = id });
        }

        /// <summary>
        /// Delete an organization
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> DeleteOrganizationAsync(int id)
        {
            var existingOrg = await _organizationService.GetOrganizationByIdAsync(id);
            if (existingOrg == null)
                return NotFound(new { message = "Organization not found" });

            await _organizationService.DeleteOrganizationAsync(id);

            return Ok(new { message = "Organization deleted successfully", orgId = id });
        }
    }
}
