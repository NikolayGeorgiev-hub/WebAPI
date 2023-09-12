using Application.Data.Models.Discounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> discount)
    {
        discount
            .HasKey(x => x.Id);

        discount
            .Property(x => x.Description)
            .IsRequired()
            .IsUnicode()
            .HasMaxLength(500);

        discount
            .Property(x => x.Percentage)
            .IsRequired()
            .HasColumnType<decimal>("decimal")
            .HasPrecision(10,2);

        discount
            .Property(x => x.IsActive)
            .IsRequired();
    }
}
