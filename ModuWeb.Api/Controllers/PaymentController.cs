using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModuWeb.Api.Models;
using ModuWeb.Api.Models.Transactions;

namespace ModuWeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Transaction transaction)
        {
            transaction.TransactionId = Guid.NewGuid();
            transaction.Status = "Initiated";
            transaction.CreatedAt = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return Ok(transaction);
        }

        // READ
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Transaction updated)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();
            transaction.Status = updated.Status;
            transaction.Amount = updated.Amount;
            transaction.Currency = updated.Currency;
            transaction.PaymentProvider = updated.PaymentProvider;
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return Ok(transaction);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // STRIPE PAYMENT INTENT ENDPOINT
        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            // TODO: Move key to config/secret store
            Stripe.StripeConfiguration.ApiKey = "sk_test_your_secret_key"; // Replace with your Stripe secret key

            var options = new Stripe.PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // Stripe expects amount in the smallest currency unit
                Currency = request.Currency ?? "sek",
                Metadata = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "orderId", request.OrderId }
                }
            };
            var service = new Stripe.PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }
    }

    public class CreatePaymentIntentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
