using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.LoginDtos;

namespace ModuWeb.Api.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ApplicationDbContext context, ILogger<LoginController> logger)
        {
            _context = context;
            _logger = logger;
        }
        // POST: api/login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for username: {UserName}", loginDto.UserName);
            
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: Username not found: {UserName}", loginDto.UserName);
                    return Unauthorized("Invalid username or password.");
                }

                var hasher = new PasswordHasher<ApplicationUser>();
                var result = hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    _logger.LogInformation("Successful login for user: {UserName} (ID: {UserId})", user.UserName, user.Id);
                    // TODO: Generate and return JWT or session info as needed
                    return Ok(new { message = "Login successful", userId = user.Id, userName = user.UserName });
                }
                else
                {
                    _logger.LogWarning("Login failed: Invalid password for username: {UserName}", loginDto.UserName);
                    return Unauthorized("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {UserName}", loginDto.UserName);
                return StatusCode(500, "An error occurred during login");
            }
        }
    }
}