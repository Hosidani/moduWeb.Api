using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.ResourceSystem;
using ModuWeb.Api.Models.ResourceSystem.ResourceDtos;


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
                Content = dto.Content
            };

            _context.Add(resource);
            await _context.SaveChangesAsync();

            var result = new ResourceDto
            {
                Id = resource.Id,
                Content = resource.Content
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
                Content = resource.Content
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
                    Content = resource.Content
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

            resource.Content = dto.Content;

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