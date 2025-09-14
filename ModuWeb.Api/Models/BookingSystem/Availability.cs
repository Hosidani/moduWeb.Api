using System;

namespace ModuWeb.Api.Models.BookingSystem
{
    public class Availability
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
        public Resource Resource { get; set; } = null!;
        public DayOfWeek DayOfWeek { get; set; } // Måndag, Tisdag, etc.
        public TimeSpan StartTime { get; set; }  // Ex: 08:00
        public TimeSpan EndTime { get; set; }    // Ex: 17:00
        public bool IsActive { get; set; } = true;
    }
}