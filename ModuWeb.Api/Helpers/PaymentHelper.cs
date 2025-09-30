using System.Collections.Generic;
using System.Linq;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.Reservations;

namespace ModuWeb.Api.Helpers
{
    public static class PaymentHelper
    {
        // Get all reservations for a given orderId
        public static List<StockReservation> GetReservationsForOrder(ApplicationDbContext context, string orderId)
        {
            return context.StockReservations.Where(r => r.OrderId == orderId).ToList();
        }

        // Initiate order: set OrderId for all reservations in session and calculate total
        public static string InitiateOrder(ApplicationDbContext context, string sessionId)
        {
            var reservations = context.StockReservations
                .Where(r => r.SessionId == sessionId && r.OrderId == null)
                .ToList();
            if (!reservations.Any())
                throw new System.Exception("No reservations found for session.");

            decimal totalAmount = 0;
            foreach (var res in reservations)
            {
                var resource = context.Resources.Find(res.ProductId);
                if (resource != null)
                {
                    var product = System.Text.Json.JsonSerializer.Deserialize<ModuWeb.Api.Models.Products.ProductDto>(resource.Content);
                    if (product != null)
                        totalAmount += product.Price * res.Quantity;
                }
            }
            var orderId = System.Guid.NewGuid().ToString();
            foreach (var res in reservations)
            {
                res.OrderId = orderId;
            }
            context.SaveChanges();
            System.Console.WriteLine($"Order initiated: OrderId={orderId}, Total={totalAmount}");
            return orderId;
        }
    }
}
