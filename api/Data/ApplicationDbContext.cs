using api.Models;
using Microsoft.EntityFrameworkCore;



namespace MyApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Resident> Resident { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Space> Space { get; set; }
        public DbSet<SpaceRule> SpaceRule { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        } 
    }
}

