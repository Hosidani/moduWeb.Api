using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models.ResourceSystem;

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
    }
}