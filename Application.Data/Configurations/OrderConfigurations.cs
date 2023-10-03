using Application.Data.Models.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Data.Configurations;

public class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> order)
    {
        order
            .HasKey(x => x.Id);

        order
            .HasOne(x => x.Details)
            .WithOne()
            .HasForeignKey<Order>(x => x.DetailsId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
