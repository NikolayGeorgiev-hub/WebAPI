using Application.Data.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions options) 
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        this.ConfigureApplicationRelations(builder);
    }

    private void ConfigureApplicationRelations(ModelBuilder builder)
         => builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

}
