using System;

namespace ModuWeb.Api.Models.BookingSystem.ResourceDtos
{
    public class ResourceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? Capacity { get; set; }
        public TimeSpan? DefaultBookingLength { get; set; }
        public decimal? Price { get; set; }
    }
}