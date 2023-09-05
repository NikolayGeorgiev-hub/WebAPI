using Application.Data.Models.Orders;
using Application.Data.Models.Products;
using Application.Data.Models.Ratings;
using Microsoft.AspNetCore.Identity;

namespace Application.Data.Models.Users;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public ICollection<IdentityUserRole<Guid>> Roles { get; set; }

    public ICollection<Product> Products { get; set; }

    public ICollection<Rating> Ratings { get; set; }

    public ICollection<Order> Orders { get; set; }
}
