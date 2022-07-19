using Hubtel.eCommerce.Cart.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cartt = Hubtel.eCommerce.Cart.Api.Models.Cart;
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

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cartt> Carts { get; set; }

        //public DbSet<Hubtel.eCommerce.Cart.Api.Models.Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            //add index
            modelBuilder.Entity<Item>()
            .HasIndex(b => b.CategoryId);

            //soft deleting 
            modelBuilder.Entity<Item>().HasQueryFilter(p => p.DeletedAt == null);
            modelBuilder.Entity<Category>().HasQueryFilter(p => p.DeletedAt == null);

            //seed data
            #region CategoriesSeed

            modelBuilder.Entity<Category>().HasData(new Category { Id = 1, Name = "Cereals and Grains" });
            modelBuilder.Entity<Category>().HasData(new Category { Id = 2, Name = "Beers and Wine" });
            #endregion

            #region ProductsSeed

            modelBuilder.Entity<Item>().HasData(new Item { Id= 1, Name = "Royal Aroma Long Grain Rice - 4kg", 
                UnitPrice = 39.99, CategoryId = 1 });
            modelBuilder.Entity<Item>().HasData(new Item {Id= 2, Name = "Millicent Vietnamese Fragrance Rice - 50kg", 
                UnitPrice = 305, CategoryId = 1 });

            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 3,
                Name = "Baileys Delight Irish Cream - 750ml",
                UnitPrice = 44.63,
                CategoryId = 2
            });
            modelBuilder.Entity<Item>().HasData(new Item
            {
                Id = 4,
                Name = "Johnnie Walker Black Label Blended Scotch Whisky - 1L",
                UnitPrice = 312.70,
                CategoryId = 2
            });

            #endregion

            #region UsersSeed
            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = "807ba6c0-e845-4695-847e-92edca9d66db",
                UserName = "macleash90@gmail.com",
                FullName = "Fred Maclean",
                NormalizedUserName = "MACLEASH90@GMAIL.COM",
                EmailConfirmed = true,
                PhoneNumber = "0244498245",
                Email = "macleash90@gmail.com",
                NormalizedEmail = "MACLEASH90@GMAIL.COM",
                AccessFailedCount = 0,
                PhoneNumberConfirmed = true,
                SecurityStamp = "CJ2EVAYDU6HKQPMFCY7A3ROLDIQNWNRM",
                ConcurrencyStamp = "073d68a5-ad7d-4312-9b25-d86d1a604329",
                //password = 111111
                PasswordHash = "AQAAAAEAACcQAAAAEG1QZ0GGItUjxtR2OYAOTlCJuKomp07mRHc31eMd6n7Dj2PMxMY2nJs7BMDEeOGlnw==",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            });

            modelBuilder.Entity<AppUser>().HasData(new AppUser
            {
                Id = "4880beda-d995-4d28-a6ab-0a3b510aea63",
                UserName = "amen@gmail.com",
                FullName = "Amen",
                NormalizedUserName = "AMEN@GMAIL.COM",
                EmailConfirmed = true,
                PhoneNumber = "0552916603",
                Email = "amen@gmail.com",
                NormalizedEmail = "AMEN@GMAIL.COM",
                AccessFailedCount = 0,
                PhoneNumberConfirmed = true,
                SecurityStamp = "CJ2EVAYDU6HKQPMFCY7A3ROLDIQNWNRM",
                ConcurrencyStamp = "073d68a5-ad7d-4312-9b25-d86d1a604329",
                //password = 111111
                PasswordHash = "AQAAAAEAACcQAAAAEG1QZ0GGItUjxtR2OYAOTlCJuKomp07mRHc31eMd6n7Dj2PMxMY2nJs7BMDEeOGlnw==",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
        });
            #endregion

            //relationships

            modelBuilder.Entity<Item>()
            .HasOne(p => p.Category)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.CategoryId)
             .HasPrincipalKey(c => c.Id);

            modelBuilder.Entity<Cartt>()
            .HasOne(p => p.User)
            .WithMany(b => b.Carts)
            .HasForeignKey(p => p.UserId)
             .HasPrincipalKey(c => c.Id)

            .OnDelete(DeleteBehavior.NoAction);
            ;

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
