using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hangfire;
using ModuWeb.Api.Models;

namespace ModuWeb.Api.Services
{
    public interface IReservationReleaseService
    {
        Task ReleaseExpiredReservationsAsync();
    }

    public class ReservationReleaseService : IReservationReleaseService
    {
        private readonly ApplicationDbContext _context;
        public ReservationReleaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ReleaseExpiredReservationsAsync()
        {
            Console.WriteLine($"[ReservationReleaseService] ReleaseExpiredReservationsAsync called at {DateTime.UtcNow:O}");
            var now = DateTime.UtcNow;
            var expiredReservations = _context.StockReservations
                .Where(r => r.Status == "active" && r.ExpiresAt < now)
                .ToList();

            Console.WriteLine($"[ReservationReleaseService] Found {expiredReservations.Count} expired reservations");

            foreach (var reservation in expiredReservations)
            {
                Console.WriteLine($"[ReservationReleaseService] Processing reservation Id={reservation.Id}, ProductId={reservation.ProductId}, Quantity={reservation.Quantity}, ExpiresAt={reservation.ExpiresAt:O}, Status={reservation.Status}");
                var resource = await _context.Resources.FindAsync(reservation.ProductId);
                if (resource == null)
                {
                    Console.WriteLine($"[ReservationReleaseService] Resource not found for ProductId={reservation.ProductId}");
                    continue;
                }
                var product = JsonSerializer.Deserialize<ModuWeb.Api.Models.Products.ProductDto>(resource.Content);
                if (product == null)
                {
                    Console.WriteLine($"[ReservationReleaseService] Product data invalid for ProductId={reservation.ProductId}");
                    continue;
                }
                // Restore stock
                var oldStock = product.Stock;
                product.Stock += reservation.Quantity;
                resource.Content = JsonSerializer.Serialize(product);
                _context.Resources.Update(resource);
                // Update reservation status
                reservation.Status = "released";
                Console.WriteLine($"[ReservationReleaseService] Released reservation: ProductId={reservation.ProductId}, Quantity={reservation.Quantity}, Stock before={oldStock}, Stock after={product.Stock}");
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"[ReservationReleaseService] Finished releasing expired reservations at {DateTime.UtcNow:O}");
        }
    }
}
