using Application.Data.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

public class OrderProductDetailsConfiguration : IEntityTypeConfiguration<OrderProductDetails>
{
    public void Configure(EntityTypeBuilder<OrderProductDetails> productDetails)
    {
        productDetails
            .HasKey(x => x.Id);

        productDetails
            .Property(x => x.ProductName)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode();

        productDetails
            .Property(x => x.Quantity)
            .IsRequired();

        productDetails
            .Property(x => x.Price)
            .IsRequired()
            .HasColumnType<decimal>("decimal")
            .HasPrecision(10, 2);

        productDetails
            .Property(x => x.TotalAmount)
            .IsRequired()
            .HasColumnType<decimal>("decimal")
            .HasPrecision(10,2);

        productDetails
            .Property(x => x.PriceWithDiscount)
            .HasColumnType<decimal?>("decimal")
            .HasPrecision(10, 2);
    }
}
