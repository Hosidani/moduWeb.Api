using System;

namespace ModuWeb.Api.Models.BookingSystem
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
        public Resource Resource { get; set; } = null!;

        public int? UserId { get; set; } // Om du har användarhantering
        // public ApplicationUser? User { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Status { get; set; } = "Booked"; // "Booked", "Cancelled", "Changed"
        public string? Notes { get; set; }
    }
}