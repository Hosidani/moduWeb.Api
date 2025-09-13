namespace ModuWeb.Api.Models.BookingSystem.ResourceDtos
{
    public class ResourceCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int? Capacity { get; set; }
        public TimeSpan? DefaultBookingLength { get; set; }
        public decimal? Price { get; set; }
    }
}