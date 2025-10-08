using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models.ResourceSystem;
using ModuWeb.Api.Models.Reservations;
using ModuWeb.Api.Models.Transactions;

namespace ModuWeb.Api.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<StockReservation> StockReservations { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    }
}