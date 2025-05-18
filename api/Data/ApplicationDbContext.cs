using api.Models;
using Microsoft.EntityFrameworkCore;



namespace MyApi.Data
{
    public class ApplicationDbContext : DbContext
    {


        public DbSet<User> User { get; set; } = null!;
        public DbSet<Reservation> Reservation { get; set; } = null!;
        public DbSet<Review> Review { get; set; } = null!;
        public DbSet<Space> Space { get; set; } = null!;
        public DbSet<SpaceRule> SpaceRule { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        } 
    }
}

