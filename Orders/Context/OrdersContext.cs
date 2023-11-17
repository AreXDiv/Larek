using Microsoft.EntityFrameworkCore;
using LarekLib.Models;

namespace Orders.Context
{
    public class OrdersContext : DbContext
    {
        public DbSet<OrdersModel> Orders { get; set; }

        public OrdersContext(DbContextOptions<OrdersContext> options) : base(options){   }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrdersModel>().ToTable("orders").HasKey(x => x.Id);
            modelBuilder.Entity<OrdersModel>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<OrdersModel>().Property(x => x.ProductId).HasColumnName("product_id");
            modelBuilder.Entity<OrdersModel>().Property(x => x.Count).HasColumnName("count");
            modelBuilder.Entity<OrdersModel>().Property(x => x.Delivery).HasColumnName("delivery");
            modelBuilder.Entity<OrdersModel>().Property(x => x.Payment).HasColumnName("payment");
            modelBuilder.Entity<OrdersModel>().Property(x => x.CustomerName).HasColumnName("customer_name");
            modelBuilder.Entity<OrdersModel>().Property(x => x.Summ).HasColumnName("summ");
        }
    }
}
