using System;
using System.Collections.Generic;

namespace ModuWeb.Api.Models.BookingSystem
{
    public class Resource
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty; // JSON-data
    }
}