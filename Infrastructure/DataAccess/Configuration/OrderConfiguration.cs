using Domain.Entities.Order;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataAccess.Configuration
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.DateOrder)
                .IsRequired();

            builder.Property(o => o.DeliveryDate)
                .IsRequired();

            builder.Property(o => o.ClientId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.Property(o => o.Price)
                .HasPrecision(18, 2);

            builder.Property(o => o.Quantity)
                .IsRequired();

            builder.Property(o => o.OrderStatus)
                .IsRequired()
                .HasMaxLength(50);

            // Configure relationship with Product (shadow navigation property)
            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
