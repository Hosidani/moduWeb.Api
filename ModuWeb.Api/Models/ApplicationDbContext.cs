using Microsoft.EntityFrameworkCore;
using ModuWeb.Api.Models.BookingSystem;

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

// If the Resource class does not exist, define it below or in a separate file
// namespace ModuWeb.Api.Models
// {
//     public class Resource
//     {
//         public int Id { get; set; }
//         // Add other properties as needed
//     }
// }