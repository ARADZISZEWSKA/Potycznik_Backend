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

            // Relacja Product -> Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // Relacja Category -> ParentCategory 
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany()
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seedowanie danych
            modelBuilder.Entity<Category>().HasData(

                new Category { Id = 1, Name = "Bar" },
                new Category { Id = 2, Name = "Kuchnia" },
                new Category { Id = 3, Name = "Chemia" },

                // Podkategorie Bar
                new Category { Id = 4, Name = "Alkohol", ParentCategoryId = 1 },
                new Category { Id = 5, Name = "Owoce", ParentCategoryId = 1 },
                new Category { Id = 6, Name = "Suche", ParentCategoryId = 1 },

                // Podkategorie Alkohol -> Alkohol Bar, Butelki, Piwo
                new Category { Id = 7, Name = "Alkohol Bar", ParentCategoryId = 4 },
                new Category { Id = 8, Name = "Butelki", ParentCategoryId = 4 },
                new Category { Id = 9, Name = "Piwo", ParentCategoryId = 4 }

                );

            modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Piwo",
                CategoryId = 9, // Powiązanie z kategorią Piwo
                Quantity = 100,
                Unit = "Litr",
                Barcode = "1234567890",
                ExpiryDate = null
            },
            new Product
            {
                Id = 2,
                Name = "Wódka",
                CategoryId = 7, // Powiązanie z kategorią Wódka
                Quantity = 50,
                Unit = "Litr",
                Barcode = "9876543210",
                ExpiryDate = null
            }
        );



        }
    }
}
