using Application.Data.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> category)
    {
        category
            .HasKey(x => x.Id);

        category
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode();
    }
}
