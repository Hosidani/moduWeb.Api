using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.LoginDtos;

namespace ModuWeb.Api.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        // POST: api/login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.UserName);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            var hasher = new PasswordHasher<ApplicationUser>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

            if (result == PasswordVerificationResult.Success)
            {
                // TODO: Generate and return JWT or session info as needed
                return Ok(new { message = "Login successful", userId = user.Id, userName = user.UserName });
            }
            else
            {
                return Unauthorized("Invalid username or password.");
            }
        }
    }
}