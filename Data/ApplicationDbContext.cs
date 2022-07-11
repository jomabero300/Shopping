using Microsoft.EntityFrameworkCore;
using TSShopping.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace TSShopping.Data
{
    public class ApplicationDbContext: IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Adm");

            modelBuilder.Entity<Country>()
            .HasIndex(x=>x.Name)
            .HasDatabaseName("IX_Country_Name")
            .IsUnique();

            modelBuilder.Entity<Category>()
            .HasIndex(x=>x.Name)
            .HasDatabaseName("IX_Category_Name")
            .IsUnique();

            modelBuilder.Entity<State>()
            .HasIndex("CountryId", "Name")
            .HasDatabaseName("IX_Country_State_Name")
            .IsUnique();

            modelBuilder.Entity<City>()
            .HasIndex("StateId", "Name")
            .HasDatabaseName("IX_State_City_Name")
            .IsUnique();

            modelBuilder.Entity<Product>()
            .HasIndex(p=>p.Name)
            .HasDatabaseName("IX_Product_Name")
            .IsUnique();

            modelBuilder.Entity<ProductCategory>()
            .HasIndex("ProductId","CategoryId")
            .HasDatabaseName("IX_Product_Cstegory")
            .IsUnique();
        }

        public DbSet<Category> Categories {get;set;}
        public DbSet<City> Cities {get;set;}
        public DbSet<Country> Countries {get;set;}
        public DbSet<Product> Products {get;set;}
        public DbSet<ProductCategory> ProductCategories {get;set;}
        public DbSet<ProductImage> ProductImages {get;set;}
        public DbSet<State> States {get;set;}
    }
}