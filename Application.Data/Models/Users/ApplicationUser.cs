using Microsoft.AspNetCore.Identity;

namespace Application.Data.Models.Users;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<IdentityUserRole<Guid>> Roles { get; set; }
}
