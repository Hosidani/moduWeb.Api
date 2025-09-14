using System;

namespace ModuWeb.Api.Models.BookingSystem.BookingDtos
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid ResourceId { get; set; }
        public int? UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}