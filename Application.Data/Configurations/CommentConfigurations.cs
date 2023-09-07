using Application.Data.Models.Comments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

public class CommentConfigurations : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> comment)
    {
        comment
            .HasKey(x => x.Id);

        comment
            .Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(500)
            .IsUnicode();
    }
}
