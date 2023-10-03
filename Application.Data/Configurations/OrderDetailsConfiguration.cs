using Application.Data.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

public class OrderDetailsConfiguration : IEntityTypeConfiguration<OrderDetails>
{
    public void Configure(EntityTypeBuilder<OrderDetails> orderDetails)
    {
        orderDetails
            .HasKey(x => x.Id);

        orderDetails
            .Property(x => x.TotalAmount)
            .IsRequired()
            .HasColumnType<decimal>("decimal")
            .HasPrecision(10, 2);

        orderDetails
            .Property(x => x.TotalAmountWithDiscount)
            .HasColumnType<decimal?>("decimal")
            .HasPrecision(10, 2);

        orderDetails
            .Property(x => x.Difference)
            .HasColumnType<decimal?>("decimal")
            .HasPrecision(10, 2);

        orderDetails
            .HasMany(x => x.Products)
            .WithOne()
            .HasForeignKey(x => x.OrderDetailsId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
