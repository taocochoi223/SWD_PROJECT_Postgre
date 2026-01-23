using Microsoft.AspNetCore.Mvc;
using SWD.API.Dtos;
using SWD.BLL.Interfaces;
using SWD.DAL.Models;
using System;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SWD.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;

        public AuthController(IUserService userService, IEmailService emailService, IJwtService jwtService)
        {
            _userService = userService;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Login - Authenticate user and return JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _userService.AuthenticateUserAsync(request.Email, request.Password);
            if (user == null) return Unauthorized(new { message = "Invalid email or password" });
            string jwtToken = _jwtService.GenerateToken(user);

            return Ok(new 
            { 
                token = jwtToken,
                user = new 
                { 
                    id = user.UserId.ToString(), 
                    email = user.Email,
                    name = user.FullName, 
                    role = user.Role?.RoleName ?? "USER"
                } 
            });
        }

        /// <summary>
        /// CreateAccount - Register a new user (Admin create for user)
        /// </summary>
        [HttpPost("register")]
        [HttpPost("createAccount")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
        {
            string randomPassword = Guid.NewGuid().ToString().Substring(0, 8) + "@A1";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(randomPassword);

            var newUser = new User
            {
                OrgId = request.OrgId,
                SiteId = request.SiteId,
                FullName = request.FullName ?? string.Empty,
                Email = request.Email ?? string.Empty,
                PasswordHash = hashedPassword,
                RoleId = request.RoleId,
                IsActive = true 
            };

            try
            {
                await _userService.RegisterUserAsync(newUser);

                string subject = "Welcome to WinMart IoT System";
                string body = $"<h3>Xin chào {request.FullName},</h3>" +
                              $"<p>Tài khoản của bạn đã được tạo.</p>" +
                              $"<p>Email: <b>{request.Email}</b></p>" +
                              $"<p>Mật khẩu: <b>{randomPassword}</b></p>" +
                              $"<p>Vui lòng đăng nhập và đổi mật khẩu ngay.</p>";

                await _emailService.SendEmailAsync(request.Email!, subject, body);

                return Ok(new { message = "User registered and email sent successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Get Current User 
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized(new { message = "Invalid or missing token" });
                
                int userId = int.Parse(userIdClaim);

                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(new
                {
                    id = user.UserId.ToString(),
                    email = user.Email,
                    name = user.FullName,
                    role = user.Role?.RoleName ?? "USER"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Deactivate User
        /// </summary>
        [HttpPost("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                if (user.IsActive == false)
                    return BadRequest(new { message = "User is already deactivated" });

                await _userService.DeactivateUserAsync(userId);

                return Ok(new { message = "User deactivated successfully", userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error: " + ex.Message });
            }
        }
    }
}