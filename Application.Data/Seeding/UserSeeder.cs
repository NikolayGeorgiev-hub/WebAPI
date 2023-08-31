using Application.Data.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using static Application.Common.Constants;

namespace Application.Data.Seeding;

public class UserSeeder : ISeeder
{
    public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        int usersCount = userManager.Users.Count();

        if (usersCount == 0)
        {
            IdentityResult result = await userManager.CreateAsync(new ApplicationUser
            {
                FirstName = TestUser.FirstName,
                Email = TestUser.Email,
                UserName = TestUser.Email,
                EmailConfirmed = true,

            }, password: TestUser.Password);
        }
    }
}
