using System;

namespace ModuWeb.Api.Models.BookingSystem.AvailabilityDtos
{
    public class AvailabilityCreateDto
    {
        public Guid ResourceId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}