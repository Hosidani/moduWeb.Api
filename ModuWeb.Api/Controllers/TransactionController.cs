using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models.Transactions;
using ModuWeb.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ModuWeb.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Transaction transaction)
        {
            transaction.TransactionId = Guid.NewGuid();
            transaction.CreatedAt = DateTime.UtcNow;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = transaction.TransactionId }, transaction);
        }

        // READ ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return Ok(transactions);
        }

        // READ BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
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
            transaction.ItemsJson = updated.ItemsJson;
            transaction.OrderId = updated.OrderId;
            transaction.SessionId = updated.SessionId;
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
            return NoContent();
        }
    }
}
