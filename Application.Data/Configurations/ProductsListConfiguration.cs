using Application.Data.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

internal class ProductsListConfiguration : IEntityTypeConfiguration<ProductsList>
{
    public void Configure(EntityTypeBuilder<ProductsList> productsList)
    {
        productsList
            .HasKey(x => new { x.OrderId, x.ProductId });

        productsList
            .HasOne(x => x.Product)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        productsList
            .HasOne(x => x.Order)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
