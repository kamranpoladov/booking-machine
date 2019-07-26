using BookingMachine.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingMachine.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }
        
        public DbSet<Computer> Computers { get; set; }
        public DbSet<Queue> Queues { get; set; }
    }
}