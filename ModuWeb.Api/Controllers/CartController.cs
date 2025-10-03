using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ModuWeb.Api.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.Products;
using ModuWeb.Api.Models.CartSystem;

//TO DO: Refactor to CRUD
namespace ModuWeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
    private readonly ApplicationDbContext _context;
    private readonly CartHelper _cartHelper;
        private const string CartSessionKey = "Cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
            _cartHelper = new CartHelper(_context);
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
            Console.WriteLine($"[Cart DEBUG] Cart state: {cartJson}");
        }

        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] CartAddRequest request)
        {
            var cart = GetCart();
            Console.WriteLine($"[Cart DEBUG] AddToCart called: ProductId={request.ProductId}, Quantity={request.Quantity}");
            var existing = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (existing != null)
            {
                existing.Quantity += request.Quantity;
                Console.WriteLine($"[Cart DEBUG] Updated quantity for ProductId={request.ProductId}: {existing.Quantity}");
            }
            else
            {
                // Fetch product details from DB
                var resource = _context.Resources.Find(request.ProductId);
                string name = "";
                int stock = 0;
                if (resource != null)
                {
                    try
                    {
                        var product = JsonSerializer.Deserialize<ProductDto>(resource.Content);
                        if (product != null)
                        {
                            name = product.Name ?? "";
                            stock = product.Stock;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cart DEBUG] Error deserializing product: {ex.Message}");
                    }
                }
                cart.Add(new CartItem {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Stock = stock,
                    Name = name
                });
                Console.WriteLine($"[Cart DEBUG] Added new item to cart: ProductId={request.ProductId}, Quantity={request.Quantity}, Name={name}, Stock={stock}");
            }
            SaveCart(cart);
            return Ok(cart);
        }

        [HttpPost("remove")]
        public IActionResult RemoveFromCart([FromBody] CartRemoveRequest request)
        {
            var cart = GetCart();
            Console.WriteLine($"[Cart DEBUG] RemoveFromCart called: ProductId={request.ProductId}");
            var item = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (item != null)
            {
                cart.Remove(item);
                Console.WriteLine($"[Cart DEBUG] Removed item from cart: ProductId={request.ProductId}");
            }
            else
            {
                Console.WriteLine($"[Cart DEBUG] Item not found in cart: ProductId={request.ProductId}");
            }
            SaveCart(cart);
            return Ok(cart);
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveStock()
        {
            var cart = GetCart();
            Console.WriteLine($"[Cart DEBUG] ReserveStock called");
            var errors = new List<object>();
            var updatedResources = new List<ModuWeb.Api.Models.ResourceSystem.Resource>();
            var reservations = new List<ModuWeb.Api.Models.Reservations.StockReservation>();
            var now = DateTime.UtcNow;
            var sessionId = HttpContext.Session.Id;
            Console.WriteLine($"[Cart DEBUG] ReserveStock sessionId={sessionId}");
            foreach (var item in cart)
            {
                var resource = await _context.Resources.FindAsync(item.ProductId);
                if (resource == null)
                {
                    errors.Add(new { item.ProductId, error = "Product not found" });
                    Console.WriteLine($"[Cart DEBUG] Product not found: ProductId={item.ProductId}");
                    continue;
                }
                var product = JsonSerializer.Deserialize<ProductDto>(resource.Content);
                if (product == null)
                {
                    errors.Add(new { item.ProductId, error = "Product data invalid" });
                    Console.WriteLine($"[Cart DEBUG] Product data invalid: ProductId={item.ProductId}");
                    continue;
                }
                if (item.Quantity > product.Stock)
                {
                    errors.Add(new { item.ProductId, error = $"Not enough stock. Requested: {item.Quantity}, Available: {product.Stock}" });
                    Console.WriteLine($"[Cart DEBUG] Not enough stock: ProductId={item.ProductId}, Requested={item.Quantity}, Available={product.Stock}");
                    continue;
                }
                // Create reservation
                var reservation = new ModuWeb.Api.Models.Reservations.StockReservation
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SessionId = sessionId,
                    ReservedAt = now,
                    ExpiresAt = now.AddMinutes(15), // Expires in 15 min
                    Status = "active",
                    OrderId = null
                };
                reservations.Add(reservation);
                Console.WriteLine($"[Cart DEBUG] Created reservation: ProductId={item.ProductId}, Quantity={item.Quantity}, SessionId={sessionId}, ReservedAt={now}, ExpiresAt={reservation.ExpiresAt}");
                // Reduce stock in DB (reserve)
                product.Stock -= item.Quantity;
                resource.Content = JsonSerializer.Serialize(product);
                updatedResources.Add(resource);
                Console.WriteLine($"[Cart DEBUG] Reserved stock: ProductId={item.ProductId}, Reserved={item.Quantity}, Remaining={product.Stock}");
            }
            if (errors.Count > 0)
            {
                Console.WriteLine($"[Cart DEBUG] Reservation failed: {JsonSerializer.Serialize(errors)}");
                return BadRequest(new { errors });
            }

            // Save updated stock for all products
            foreach (var res in updatedResources)
            {
                _context.Resources.Update(res);
            }
            // Save reservations
            foreach (var r in reservations)
            {
                _context.StockReservations.Add(r);
            }
            await _context.SaveChangesAsync();

            Console.WriteLine($"[Cart DEBUG] Reserved stock for cart: {JsonSerializer.Serialize(cart)}");
            Console.WriteLine($"[Cart DEBUG] Saved {reservations.Count} reservations to DB");
            return Ok(new { reserved = true, cart });
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            var sessionId = HttpContext.Session.Id;
            Console.WriteLine($"[Cart DEBUG] Checkout called for SessionId={sessionId}");
            try
            {
                (string orderId, decimal totalAmount) = _cartHelper.CheckoutSession(sessionId);
                HttpContext.Session.Remove(CartSessionKey);
                Console.WriteLine($"[Cart DEBUG] Cart cleared after checkout");
                return Ok(new { success = true, orderId, totalAmount });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cart DEBUG] Checkout error: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        // Simple reservation release logic:
        // If session expires or user removes items from cart, reserved stock is not restored in DB in this demo.
        // In a real system, use a background job to restore stock for expired sessions.
    }
}
