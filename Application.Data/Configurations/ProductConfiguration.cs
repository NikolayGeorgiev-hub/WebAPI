using Application.Data.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> product)
    {
        product
            .HasKey(x => x.Id);

        product
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode();

        product
            .Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000)
            .IsUnicode();

        product
             .Property(x => x.Price)
             .IsRequired()
             .HasColumnType<decimal>("decimal")
             .HasPrecision(10, 2);

        product
            .Property(x => x.InStock)
            .IsRequired();

        product
            .Property(x => x.Quantity)
            .IsRequired();

        product
            .HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        product
            .HasOne(x => x.SubCategory)
            .WithMany()
            .HasForeignKey(x => x.SubCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        product
            .HasMany(x => x.Ratings)
            .WithOne()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        product
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
