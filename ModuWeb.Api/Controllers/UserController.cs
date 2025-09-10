using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.CRUDUserDtos;

namespace ModuWeb.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ReadUserDto>> CreateUser(CreateUserDto userDto)
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            var user = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                // other properties
            };
            user.PasswordHash = hasher.HashPassword(user, userDto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = new ReadUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email

            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, result);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadUserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return new ReadUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadUserDto>>> GetUsers()
        {
            return await _context.Users
                .Select(u => new ReadUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email
                })
                .ToListAsync();
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.UserName = dto.UserName;
            user.Email = dto.Email;

            // If updating password, hash it
            if (!string.IsNullOrEmpty(dto.Password))
            {
                var hasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = hasher.HashPassword(user, dto.Password);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully" });
        }
        
        // POST: api/users/delete
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDto dto)
        {
            var user = await _context.Users.FindAsync(dto.Id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
    }
}