using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.Products;
using ModuWeb.Api.Models.CartSystem;

namespace ModuWeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> AddToCart([FromBody] CartAddRequest request)
        {
            var cart = GetCart();
            var resource = await _context.Resources.FindAsync(request.ProductId);
            if (resource == null)
                return NotFound(new { message = "Product not found" });
            var product = JsonSerializer.Deserialize<ProductDto>(resource.Content);
            if (product == null)
                return NotFound(new { message = "Product data invalid" });
            var existing = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (existing != null)
            {
                existing.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(new CartItem {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Stock = product.Stock,
                    Name = product.Name
                });
            }
            SaveCart(cart);
            return Ok(cart);
        }

        [HttpPost("remove")]
        public IActionResult RemoveFromCart([FromBody] CartRemoveRequest request)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (item != null)
            {
                cart.Remove(item);
            }
            SaveCart(cart);
            return Ok(cart);
        }

        [HttpPost("reserve")]
        public IActionResult ReserveStock()
        {
            var cart = GetCart();
            // For demo: just print reservation
            Console.WriteLine($"[Cart DEBUG] Reserving stock for cart: {JsonSerializer.Serialize(cart)}");
            return Ok(new { reserved = true, cart });
        }

        [HttpPost("checkout")]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            // For demo: just print checkout
            Console.WriteLine($"[Cart DEBUG] Checkout for cart: {JsonSerializer.Serialize(cart)}");
            // Optionally clear cart after checkout
            HttpContext.Session.Remove(CartSessionKey);
            return Ok(new { success = true, cart });
        }

// ...existing code...
    }
}
