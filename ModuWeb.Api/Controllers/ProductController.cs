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

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var resources = await _context.Resources.ToListAsync();
            var products = resources
                .Select(r => JsonSerializer.Deserialize<ProductDto>(r.Content))
                .Where(p => p != null)
                .ToList();
            return products;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return NotFound();
            var product = JsonSerializer.Deserialize<ProductDto>(resource.Content);
            if (product == null)
                return NotFound();
            return product;
        }


        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto product)
        {
            var resource = new Resource
            {
                Id = Guid.NewGuid(),
                Content = JsonSerializer.Serialize(product)
            };
            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = resource.Id }, product);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, ProductDto product)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return NotFound();
            resource.Content = JsonSerializer.Serialize(product);
            _context.Entry(resource).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return NotFound();
            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
