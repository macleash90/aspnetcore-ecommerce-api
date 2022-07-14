using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.eCommerce.Cart.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        private readonly DbContextOptions _options;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

            _options = options;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            //add index
            modelBuilder.Entity<Product>()
            .HasIndex(b => b.CategoryId);

            //soft deleting 
            modelBuilder.Entity<Product>().HasQueryFilter(p => p.DeletedAt == null);
            modelBuilder.Entity<Category>().HasQueryFilter(p => p.DeletedAt == null);

            //seed data
            #region CategoriesSeed

            modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "Cereals and Grains" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 2, Name = "Beers and Wine" });
            #endregion

            #region ProductsSeed

            modelBuilder.Entity<Product>().HasData(new Product { Id= 1, Name = "Royal Aroma Long Grain Rice - 4kg", 
                UnitPrice = 39.99, CategoryId = 1 });
            modelBuilder.Entity<Product>().HasData(new Product {Id= 2, Name = "Millicent Vietnamese Fragrance Rice - 50kg", 
                UnitPrice = 305, CategoryId = 1 });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 3,
                Name = "Baileys Delight Irish Cream - 750ml",
                UnitPrice = 44.63,
                CategoryId = 2
            });
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = 4,
                Name = "Johnnie Walker Black Label Blended Scotch Whisky - 1L",
                UnitPrice = 312.70,
                CategoryId = 2
            });

            #endregion

            //relationships

            modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.CategoryId)
             .HasPrincipalKey(c => c.Id);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Startup.DefaultConnection);
            }
        }

    }
}
