using LarekLib.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Context
{
    public class CatalogContext : DbContext
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>().ToTable("brand").HasKey(x => x.Id);
            modelBuilder.Entity<Brand>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<Brand>().Property(x => x.Name).HasColumnName("name");

            modelBuilder.Entity<Category>().ToTable("category").HasKey(x => x.Id);
            modelBuilder.Entity<Category>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<Category>().Property(x => x.Name).HasColumnName("name");

            modelBuilder.Entity<Product>().ToTable("product").HasKey(x => x.Id);
            modelBuilder.Entity<Product>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<Product>().Property(x => x.Name).HasColumnName("name");
            modelBuilder.Entity<Product>().Property(x => x.Count).HasColumnName("count");
            modelBuilder.Entity<Product>().Property(x => x.Price).HasColumnName("price");
            modelBuilder.Entity<Product>().Property(x => x.BrandId).HasColumnName("brandid");
            modelBuilder.Entity<Product>().HasOne(x => x.Brand);
            modelBuilder.Entity<Product>().Property(x => x.CategoryId).HasColumnName("categoryid");
            modelBuilder.Entity<Product>().HasOne(x => x.Category);
        }
    }
}
