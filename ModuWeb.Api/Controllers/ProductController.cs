using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.Products;
using ModuWeb.Api.Models.ResourceSystem;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModuWeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private void DebugLog(string message)
        {
            Console.WriteLine($"[ProductController DEBUG] {DateTime.Now:O} - {message}");
        }

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
            DebugLog("ProductController initialized");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts([FromQuery] string? category = null)
        {
            DebugLog($"GetProducts called with category: '{category}'");
            var resources = await _context.Resources.ToListAsync();
            DebugLog($"Loaded {resources.Count} resources from DB");
            var products = resources
                .Select(r => {
                    try {
                        var product = JsonSerializer.Deserialize<ProductDto>(r.Content);
                        if (product == null) DebugLog($"Resource {r.Id} deserialized to null");
                        return product == null ? null : new { id = r.Id, content = product };
                    } catch (Exception ex) {
                        DebugLog($"Deserialization error for resource {r.Id}: {ex.Message}");
                        return null;
                    }
                })
                .Where(p => p != null)
                .ToList();

            DebugLog($"Products after deserialization: {products.Count}");
            if (!string.IsNullOrWhiteSpace(category))
            {
                products = products
                    .Where(p => p.content.Categories != null && p.content.Categories.Contains(category, StringComparer.OrdinalIgnoreCase))
                    .ToList();
                DebugLog($"Products after category filter: {products.Count}");
            }
            return products;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduct(Guid id)
        {
            DebugLog($"GetProduct called with id: {id}");
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                DebugLog($"Resource not found for id: {id}");
                return NotFound();
            }
            try {
                var product = JsonSerializer.Deserialize<ProductDto>(resource.Content);
                if (product == null)
                {
                    DebugLog($"Deserialization returned null for id: {id}");
                    return NotFound();
                }
                return new { id = resource.Id, content = product };
            } catch (Exception ex) {
                DebugLog($"Deserialization error for id: {id}: {ex.Message}");
                return StatusCode(500, $"Deserialization error: {ex.Message}");
            }
        }



        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto product)
        {
            DebugLog($"CreateProduct called with product: {JsonSerializer.Serialize(product)}");
            var resource = new Resource
            {
                Id = Guid.NewGuid(),
                Content = JsonSerializer.Serialize(product)
            };
            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            DebugLog($"Product created with id: {resource.Id}");
            return CreatedAtAction(nameof(GetProduct), new { id = resource.Id }, product);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, ProductDto product)
        {
            DebugLog($"UpdateProduct called for id: {id} with product: {JsonSerializer.Serialize(product)}");
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                DebugLog($"Resource not found for update, id: {id}");
                return NotFound();
            }
            resource.Content = JsonSerializer.Serialize(product);
            _context.Entry(resource).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            DebugLog($"Product updated for id: {id}");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            DebugLog($"DeleteProduct called for id: {id}");
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                DebugLog($"Resource not found for delete, id: {id}");
                return NotFound();
            }
            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            DebugLog($"Product deleted for id: {id}");
            return NoContent();
        }
    }
}
