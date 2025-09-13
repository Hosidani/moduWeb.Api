using System;

namespace ModuWeb.Api.Models.BookingSystem.BookingDtos
{
    public class BookingCreateDto
    {
        public Guid ResourceId { get; set; }
        public int? UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}