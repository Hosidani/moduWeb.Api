using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace ModuWeb.Api.Helpers
{
    public class CartHelper
    {
        private readonly ApplicationDbContext _context;
        public CartHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public (string orderId, decimal totalAmount) CheckoutSession(string sessionId)
        {
            Console.WriteLine($"[CartHelper DEBUG] CheckoutSession called for SessionId={sessionId}");
            var reservations = _context.StockReservations
                .Where(r => r.SessionId == sessionId && r.OrderId == null)
                .ToList();
            if (!reservations.Any())
            {
                Console.WriteLine($"[CartHelper DEBUG] No reservations found for session");
                throw new Exception("No reservations found for session.");
            }
            decimal totalAmount = 0;
            foreach (var res in reservations)
            {
                var resource = _context.Resources.Find(res.ProductId);
                if (resource != null)
                {
                    var product = JsonSerializer.Deserialize<ProductDto>(resource.Content);
                    if (product != null)
                        totalAmount += product.Price * res.Quantity;
                }
            }
            var orderId = Guid.NewGuid().ToString();
            foreach (var res in reservations)
            {
                res.OrderId = orderId;
            }
            _context.SaveChanges();
            Console.WriteLine($"[CartHelper DEBUG] Order created: OrderId={orderId}, Total={totalAmount}");
            return (orderId, totalAmount);
        }
    }
}
