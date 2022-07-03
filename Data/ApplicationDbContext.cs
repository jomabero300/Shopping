using Microsoft.EntityFrameworkCore;
using TSShopping.Data.Entities;

namespace TSShopping.Data
{
    public class ApplicationDbContext: DbContext
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
        }

        public DbSet<Country> Countries {get;set;}
        public DbSet<Category> Categories {get;set;}
    }
}