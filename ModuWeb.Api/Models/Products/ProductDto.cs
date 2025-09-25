using System;
using System.Collections.Generic;

namespace ModuWeb.Api.Models.Products
{
    // DTO for product data
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SKU { get; set; }
    public List<string?> Images { get; set; } = new List<string?>();
    public List<string> Categories { get; set; } = new List<string>();
    }
}