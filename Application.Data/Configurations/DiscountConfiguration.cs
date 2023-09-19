using Application.Data.Models.Discounts;
using Application.Data.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> discount)
    {
        discount
            .HasKey(x => x.Id);

        discount
            .Property(x => x.Description)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(300);

        discount
             .Property(x => x.Percentage)
             .IsRequired()
             .HasColumnType<decimal>("decimal")
             .HasPrecision(10, 2);
    }
}
