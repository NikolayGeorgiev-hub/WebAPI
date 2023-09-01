using Application.Data.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

public class SubCategoryConfigurations : IEntityTypeConfiguration<SubCategory>
{
    public void Configure(EntityTypeBuilder<SubCategory> subCategory)
    {
        subCategory
            .HasKey(x => x.Id);

        subCategory
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode();

        subCategory
            .HasOne(x => x.Category)
            .WithMany(x => x.SubCategories)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
