using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models.BookingSystem;
using ModuWeb.Api.Models.BookingSystem.BookingDtos;
using ModuWeb.Api.Models;

namespace ModuWeb.Api.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(BookingCreateDto dto)
        {
            // Kontrollera tillgänglighet
            var availabilities = await _context.Availabilities
                .Where(a => a.ResourceId == dto.ResourceId && a.DayOfWeek == dto.StartTime.DayOfWeek && a.IsActive)
                .ToListAsync();

            bool isWithinAvailability = availabilities.Any(a =>
                dto.StartTime.TimeOfDay >= a.StartTime &&
                dto.EndTime.TimeOfDay <= a.EndTime
            );

            if (!isWithinAvailability)
                return BadRequest("Resource is not available at the requested time.");

            // Kontrollera överlappande bokningar
            bool overlaps = await _context.Bookings.AnyAsync(b =>
                b.ResourceId == dto.ResourceId &&
                b.Status == "Booked" &&
                ((dto.StartTime < b.EndTime) && (dto.EndTime > b.StartTime))
            );
            if (overlaps)
                return BadRequest("Resource is already booked at the requested time.");

            // Ensure DateTimeKind.Utc for PostgreSQL
            dto.StartTime = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc);
            dto.EndTime = DateTime.SpecifyKind(dto.EndTime, DateTimeKind.Utc);

            var booking = new Booking
            {
                ResourceId = dto.ResourceId,
                UserId = dto.UserId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = "Booked",
                Notes = dto.Notes
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var result = new BookingDto
            {
                Id = booking.Id,
                ResourceId = booking.ResourceId,
                UserId = booking.UserId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status,
                Notes = booking.Notes
            };

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, result);
        }

        // GET: api/bookings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            return new BookingDto
            {
                Id = booking.Id,
                ResourceId = booking.ResourceId,
                UserId = booking.UserId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status,
                Notes = booking.Notes
            };
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings()
        {
            return await _context.Bookings
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    ResourceId = b.ResourceId,
                    UserId = b.UserId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Status = b.Status,
                    Notes = b.Notes
                })
                .ToListAsync();
        }

        // PUT: api/bookings/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] BookingCreateDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            booking.ResourceId = dto.ResourceId;
            booking.UserId = dto.UserId;
            booking.StartTime = dto.StartTime;
            booking.EndTime = dto.EndTime;
            booking.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking updated successfully" });
        }

        // DELETE: api/bookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Booking deleted successfully" });
        }
    }
}