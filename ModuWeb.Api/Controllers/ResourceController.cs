using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models.BookingSystem;
using ModuWeb.Api.Models.BookingSystem.ResourceDtos;
using ModuWeb.Api.Models;

namespace ModuWeb.Api.Controllers
{
    [Route("api/resources")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResourceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/resources
        [HttpPost]
        public async Task<ActionResult<ResourceDto>> CreateResource(ResourceCreateDto dto)
        {
            var resource = new Resource
            {
                Name = dto.Name,
                Type = dto.Type,
                Description = dto.Description,
                IsActive = dto.IsActive,
                Capacity = dto.Capacity,
                DefaultBookingLength = dto.DefaultBookingLength,
                Price = dto.Price
            };

            _context.Add(resource);
            await _context.SaveChangesAsync();

            var result = new ResourceDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Type = resource.Type,
                Description = resource.Description,
                IsActive = resource.IsActive,
                Capacity = resource.Capacity,
                DefaultBookingLength = resource.DefaultBookingLength,
                Price = resource.Price
            };

            return CreatedAtAction(nameof(GetResource), new { id = resource.Id }, result);
        }

        // GET: api/resources/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDto>> GetResource(Guid id)
        {
            var resource = await _context.Set<Resource>().FindAsync(id);
            if (resource == null)
                return NotFound();

            return new ResourceDto
            {
                Id = resource.Id,
                Name = resource.Name,
                Type = resource.Type,
                Description = resource.Description,
                IsActive = resource.IsActive,
                Capacity = resource.Capacity,
                DefaultBookingLength = resource.DefaultBookingLength,
                Price = resource.Price
            };
        }

        // GET: api/resources
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetResources()
        {
            return await _context.Set<Resource>()
                .Select(resource => new ResourceDto
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    Type = resource.Type,
                    Description = resource.Description,
                    IsActive = resource.IsActive,
                    Capacity = resource.Capacity,
                    DefaultBookingLength = resource.DefaultBookingLength,
                    Price = resource.Price
                })
                .ToListAsync();
        }

        // PUT: api/resources/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResource(Guid id, [FromBody] ResourceCreateDto dto)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return NotFound();

            resource.Name = dto.Name;
            resource.Type = dto.Type;
            resource.Description = dto.Description;
            resource.IsActive = dto.IsActive;
            resource.Capacity = dto.Capacity;
            resource.DefaultBookingLength = dto.DefaultBookingLength;
            resource.Price = dto.Price;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Resource updated successfully" });
        }

        // DELETE: api/resources/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(Guid id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return NotFound();

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Resource deleted successfully" });
        }
    }
}