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
    }
}
