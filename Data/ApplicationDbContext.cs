using Microsoft.EntityFrameworkCore;
using ClothingWholesaleAPI.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClothingWholesaleAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Order entity for PostgreSQL
            modelBuilder.Entity<Order>(entity =>
            {
                // Table name (PostgreSQL uses lowercase by default)
                entity.ToTable("orders");

                // Primary key
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd(); // Auto increment

                // Required string properties
                entity.Property(e => e.CustomerName)
                    .HasColumnName("customer_name")
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.ShippingAddress)
                    .HasColumnName("shipping_address")
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.ContactPhone)
                    .HasColumnName("contact_phone")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.TrackingNumber)
                    .HasColumnName("tracking_number")
                    .HasMaxLength(100)
                    .IsRequired();

                // DateTime properties
                entity.Property(e => e.OrderDate)
                    .HasColumnName("order_date")
                    .HasColumnType("timestamp with time zone")
                    .IsRequired();

                entity.Property(e => e.DeliveryDate)
                    .HasColumnName("delivery_date")
                    .HasColumnType("timestamp with time zone");

                // Decimal property for money
                entity.Property(e => e.TotalAmount)
                    .HasColumnName("total_amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                // Currency with default value
                entity.Property(e => e.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .HasDefaultValue("USD");

                // Optional notes
                entity.Property(e => e.Notes)
                    .HasColumnName("notes")
                    .HasMaxLength(1000);
            });

            // Create indexes for better performance
            modelBuilder.Entity<Order>()
                .HasIndex(e => e.Status)
                .HasDatabaseName("ix_orders_status");

            modelBuilder.Entity<Order>()
                .HasIndex(e => e.OrderDate)
                .HasDatabaseName("ix_orders_order_date");

            modelBuilder.Entity<Order>()
                .HasIndex(e => e.TrackingNumber)
                .HasDatabaseName("ix_orders_tracking_number");
        }
    }
}