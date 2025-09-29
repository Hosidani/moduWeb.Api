using System;

namespace ModuWeb.Api.Models.CartSystem
{
    public class CartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }
        public string Name { get; set; }
    }
}
