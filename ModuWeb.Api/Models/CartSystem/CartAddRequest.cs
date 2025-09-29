using System;

namespace ModuWeb.Api.Models.CartSystem
{
    public class CartAddRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
