using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModuWeb.Api.Models.Payments;
using ModuWeb.Api.Models;

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
    }
}
