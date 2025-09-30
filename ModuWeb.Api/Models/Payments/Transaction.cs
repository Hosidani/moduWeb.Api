using System;

namespace ModuWeb.Api.Models.Payments
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentProvider { get; set; }
        public string Status { get; set; } // Initiated, Success, Failed, Refunded
        public DateTime CreatedAt { get; set; }
    }
}
