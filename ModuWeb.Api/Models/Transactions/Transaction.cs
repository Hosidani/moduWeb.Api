using System;
using System.ComponentModel.DataAnnotations;

namespace ModuWeb.Api.Models.Transactions
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "SEK";
        public string PaymentProvider { get; set; } = string.Empty;
        public string Status { get; set; } = "pending"; // accepted/rejected/pending
        public string ItemsJson { get; set; } = string.Empty; // JSON with buy items
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
