using Microsoft.EntityFrameworkCore;
using Potycznik_Backend.Models;

namespace Potycznik_Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //tabele
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<InventoryRecord> InventoryRecords { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Loss> Losses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacja Inventory -> InventoryRecords
            modelBuilder.Entity<InventoryRecord>()
                .HasOne(ir => ir.Inventory)
                .WithMany(i => i.InventoryRecords)
                .HasForeignKey(ir => ir.InventoryId);

            // Relacja Product -> InventoryRecords
            modelBuilder.Entity<InventoryRecord>()
                .HasOne(ir => ir.Product)
                .WithMany(p => p.InventoryRecords)
                .HasForeignKey(ir => ir.ProductId);

            // Pozostałe relacje
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany()
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
