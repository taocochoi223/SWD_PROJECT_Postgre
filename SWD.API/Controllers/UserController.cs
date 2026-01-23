using Microsoft.AspNetCore.Authorization; // Add this
using Microsoft.AspNetCore.Mvc;
using SWD.API.Dtos;
using SWD.BLL.Interfaces;

namespace SWD.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userService.GetAllUsersAsync();

            var userDtos = users.Select(user => new
            {
                userId = user.UserId,
                email = user.Email,
                fullName = user.FullName,
                roleId = user.RoleId,
                roleName = user.Role?.RoleName,
                orgId = user.OrgId,
                siteId = user.SiteId,
                siteName = user.Site?.Name,
                isActive = user.IsActive
            });

            return Ok(userDtos);
        }

        /// <summary>
        /// Get User By ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                userId = user.UserId,
                email = user.Email,
                fullName = user.FullName,
                roleId = user.RoleId,
                roleName = user.Role?.RoleName,
                orgId = user.OrgId,
                siteId = user.SiteId,
                isActive = user.IsActive
            });
        }

        /// <summary>
        /// Update User
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,ADMIN,Manager,MANAGER")]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UpdateUserDto request)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Update fields
            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;
            
            if (request.SiteId.HasValue)
                user.SiteId = request.SiteId.Value;

            await _userService.UpdateUserAsync(user);

            return Ok(new { message = "User updated successfully", userId = user.UserId });
        }

        /// <summary>
        /// Activate User
        /// </summary>
        [HttpPut("{id}/activate")]
        [Authorize(Roles = "Admin,ADMIN")]
        public async Task<IActionResult> ActivateUserAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });

            if (user.IsActive == true)
                return BadRequest(new { message = "User is already active" });

            user.IsActive = true;
            await _userService.UpdateUserAsync(user);

            return Ok(new { message = "User activated successfully", userId = id });
        }
    }

}
