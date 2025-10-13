using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models.ResourceSystem;
using ModuWeb.Api.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ModuWeb.Api.Models.ResourceSystem.ResourceDtos;
using Microsoft.Extensions.Logging;

namespace ModuWeb.Api.Controllers
{
    [Route("api/bookingtemplates")]
    [ApiController]
    public class BookingTemplateController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingTemplateController> _logger;

        public BookingTemplateController(ApplicationDbContext context, ILogger<BookingTemplateController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/bookingtemplates
        [HttpPost]
        public async Task<ActionResult<ResourceDto>> Create([FromBody] ResourceCreateDto dto)
        {
            _logger.LogInformation("POST /api/bookingtemplates called with content: {Content}", dto.Content);
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

            _logger.LogInformation("Resource created with Id: {Id}", resource.Id);
            return CreatedAtAction(nameof(GetById), new { id = resource.Id }, result);
        }

        // GET: api/bookingtemplates/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceDto>> GetById(Guid id)
        {
            _logger.LogInformation("GET /api/bookingtemplates/{Id} called", id);
            var resource = await _context.Set<Resource>().FindAsync(id);
            if (resource == null)
            {
                _logger.LogWarning("Resource with Id {Id} not found", id);
                return NotFound();
            }

            return new ResourceDto
            {
                Id = resource.Id,
                Content = resource.Content
            };
        }

        // GET: api/bookingtemplates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResourceDto>>> GetAll()
        {
            _logger.LogInformation("GET /api/bookingtemplates called");
            return await _context.Set<Resource>()
                .Select(resource => new ResourceDto
                {
                    Id = resource.Id,
                    Content = resource.Content
                })
                .ToListAsync();
        }

        // PUT: api/bookingtemplates/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ResourceCreateDto dto)
        {
            _logger.LogInformation("PUT /api/bookingtemplates/{Id} called with content: {Content}", id, dto.Content);
            var resource = await _context.Set<Resource>().FindAsync(id);
            if (resource == null)
            {
                _logger.LogWarning("Resource with Id {Id} not found for update", id);
                return NotFound();
            }

            resource.Content = dto.Content;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Resource with Id {Id} updated", id);
            return Ok(new { message = "Resource updated successfully" });
        }

        // DELETE: api/bookingtemplates/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("DELETE /api/bookingtemplates/{Id} called", id);
            var resource = await _context.Set<Resource>().FindAsync(id);
            if (resource == null)
            {
                _logger.LogWarning("Resource with Id {Id} not found for delete", id);
                return NotFound();
            }

            _context.Remove(resource);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Resource with Id {Id} deleted", id);
            return Ok(new { message = "Resource deleted successfully" });
        }
    }
}
