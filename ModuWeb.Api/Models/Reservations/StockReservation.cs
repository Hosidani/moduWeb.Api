using System;

namespace ModuWeb.Api.Models.Reservations
{
    public class StockReservation
    {
        public Guid Id { get; set; } // Unik reservation
        public Guid ProductId { get; set; } // Koppling till Resource/produkt
        public int Quantity { get; set; } // Antal reserverat
        public string SessionId { get; set; } // Session eller UserId
        public DateTime ReservedAt { get; set; } // När reservationen skapades
        public DateTime ExpiresAt { get; set; } // När reservationen går ut
        public string Status { get; set; } // "active", "released", "completed"
        public string? OrderId { get; set; } // Sätts vid slutfört köp
    }
}