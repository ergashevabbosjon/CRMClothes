using Microsoft.EntityFrameworkCore;
using ClothingWholesaleAPI.Models; // Modellaringiz joylashgan joy

namespace ClothingWholesaleAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Ma'lumotlar bazasidagi jadvallarga mos DbSet'lar
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}