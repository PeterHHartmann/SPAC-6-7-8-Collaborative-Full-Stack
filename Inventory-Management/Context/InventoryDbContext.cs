using Inventory_Management.Model;
using Inventory_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management.Context
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationship between Order and Customer
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship between OrderItem and Order
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationship between OrderItem and Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship between Product and Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship between InventoryTransaction and Product
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(it => it.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationship between InventoryTransaction and User
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Customer)
                .WithMany(u => u.InventoryTransactions)
                .HasForeignKey(it => it.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }   
    }
}
