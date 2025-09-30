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
    }
}
