using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models.BookingSystem;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.BookingSystem.AvailabilityDtos;

namespace ModuWeb.Api.Controllers
{
    [Route("api/availabilities")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<AvailabilityDto>> Create([FromBody] AvailabilityCreateDto dto)
        {
            var availability = new Availability
            {
                ResourceId = dto.ResourceId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsActive = dto.IsActive
            };
            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();

            var result = new AvailabilityDto
            {
                Id = availability.Id,
                ResourceId = availability.ResourceId,
                DayOfWeek = availability.DayOfWeek,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                IsActive = availability.IsActive
            };
            return CreatedAtAction(nameof(Get), new { id = availability.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AvailabilityDto>> Get(Guid id)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability == null) return NotFound();
            return new AvailabilityDto
            {
                Id = availability.Id,
                ResourceId = availability.ResourceId,
                DayOfWeek = availability.DayOfWeek,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime,
                IsActive = availability.IsActive
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AvailabilityDto>>> GetAll()
        {
            return await _context.Availabilities
                .Select(a => new AvailabilityDto
                {
                    Id = a.Id,
                    ResourceId = a.ResourceId,
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsActive = a.IsActive
                })
                .ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AvailabilityCreateDto dto)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability == null) return NotFound();

            availability.ResourceId = dto.ResourceId;
            availability.DayOfWeek = dto.DayOfWeek;
            availability.StartTime = dto.StartTime;
            availability.EndTime = dto.EndTime;
            availability.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Availability updated" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability == null) return NotFound();

            _context.Availabilities.Remove(availability);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Availability deleted" });
        }
    }
}