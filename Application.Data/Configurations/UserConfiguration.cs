using Application.Data.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> user)
    {
        user
            .Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode();

        user
             .HasMany(x => x.Roles)
             .WithOne()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);

        user
            .HasMany(x => x.Products)
            .WithOne()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        user
            .HasMany(x => x.Ratings)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        user
            .HasMany(x => x.Orders)
            .WithOne()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
