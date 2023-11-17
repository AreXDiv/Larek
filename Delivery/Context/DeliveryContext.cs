using Microsoft.EntityFrameworkCore;
using LarekLib.Models;

namespace Delivery.Context
{
    public class DeliveryContext : DbContext
    {
        public DbSet<DeliveryModel> Deliveries { get; set; }

        public DeliveryContext(DbContextOptions<DeliveryContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeliveryModel>().ToTable("delivery").HasKey(x => x.Id);
            modelBuilder.Entity<DeliveryModel>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<DeliveryModel>().Property(x => x.OrderId).HasColumnName("order_id");
            modelBuilder.Entity<DeliveryModel>().Property(x => x.DeliveryLocation).HasColumnName("delivery_location");
            modelBuilder.Entity<DeliveryModel>().Property(x => x.DateOrder).HasColumnName("date_order");
            modelBuilder.Entity<DeliveryModel>().Property(x => x.IsCollected).HasColumnName("is_collected");
            modelBuilder.Entity<DeliveryModel>().Property(x => x.IsDelivered).HasColumnName("is_delivered");
            modelBuilder.Entity<DeliveryModel>().Property(x => x.Phone).HasColumnName("phone");
        }
    }
}
