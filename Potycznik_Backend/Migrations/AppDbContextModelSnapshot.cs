﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Potycznik_Backend.Data;

#nullable disable

namespace Potycznik_Backend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Potycznik_Backend.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentCategoryId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentCategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Bar"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Kuchnia"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Chemia"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Alkohol",
                            ParentCategoryId = 1
                        },
                        new
                        {
                            Id = 5,
                            Name = "Owoce",
                            ParentCategoryId = 1
                        },
                        new
                        {
                            Id = 6,
                            Name = "Suche",
                            ParentCategoryId = 1
                        },
                        new
                        {
                            Id = 7,
                            Name = "Alkohol Bar",
                            ParentCategoryId = 4
                        },
                        new
                        {
                            Id = 8,
                            Name = "Butelki",
                            ParentCategoryId = 4
                        },
                        new
                        {
                            Id = 9,
                            Name = "Piwo",
                            ParentCategoryId = 4
                        });
                });

            modelBuilder.Entity("Potycznik_Backend.Models.Inventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Inventories");
                });

            modelBuilder.Entity("Potycznik_Backend.Models.InventoryRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("InventoryId")
                        .HasColumnType("int");

                    b.Property<decimal>("PreviousQuantity")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("InventoryId");

                    b.HasIndex("ProductId");

                    b.ToTable("InventoryRecords");
                });

            modelBuilder.Entity("Potycznik_Backend.Models.Loss", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Losses");
                });

            modelBuilder.Entity("Potycznik_Backend.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Barcode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("Unit")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Barcode = "1234567890",
                            CategoryId = 9,
                            Name = "Piwo",
                            Quantity = 100m,
                            Unit = "Litr"
                        },
                        new
                        {
                            Id = 2,
                            Barcode = "9876543210",
                            CategoryId = 7,
                            Name = "Wódka",
                            Quantity = 50m,
                            Unit = "Litr"
                        });
                });

            modelBuilder.Entity("Potycznik_Backend.Models.Category", b =>
                {
                    b.HasOne("Potycznik_Backend.Models.Category", "ParentCategory")
                        .WithMany()
                        .HasForeignKey("ParentCategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ParentCategory");
                });

            modelBuilder.Entity("Potycznik_Backend.Models.InventoryRecord", b =>
                {
                    b.HasOne("Potycznik_Backend.Models.Inventory", null)
                        .WithMany("InventoryRecords")
                        .HasForeignKey("InventoryId");

                    b.HasOne("Potycznik_Backend.Models.Product", "Product")
                        .WithMany("InventoryRecords")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Potycznik_Backend.Models.Loss", b =>
                {
                    b.HasOne("Potycznik_Backend.Models.Product", null)
                        .WithMany("Losses")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Potycznik_Backend.Models.Product", b =>
                {
                    b.HasOne("Potycznik_Backend.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Potycznik_Backend.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Potycznik_Backend.Models.Inventory", b =>
                {
                    b.Navigation("InventoryRecords");
                });

            modelBuilder.Entity("Potycznik_Backend.Models.Product", b =>
                {
                    b.Navigation("InventoryRecords");

                    b.Navigation("Losses");
                });
#pragma warning restore 612, 618
        }
    }
}
