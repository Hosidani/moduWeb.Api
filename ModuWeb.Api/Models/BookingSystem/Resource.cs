using System;
using System.Collections.Generic;

namespace ModuWeb.Api.Models.BookingSystem
{
    public class Resource
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;           // "Rum 101", "Massage 60 min"
        public string Type { get; set; } = string.Empty;           // "Room", "Person", "Service"
        public string? Description { get; set; }                   // Fri text
        public bool IsActive { get; set; } = true;                 // Om resursen är bokningsbar

        public int? Capacity { get; set; }                         // Ex. 20 personer för ett rum
        public TimeSpan? DefaultBookingLength { get; set; }        // Ex. 60 min för en tjänst
        public decimal? Price { get; set; }                        // Om det finns prissättning

        // Navigation properties som kanske behövs senare
        //public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
        //public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}