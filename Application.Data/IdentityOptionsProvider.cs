using Microsoft.AspNetCore.Identity;

namespace Application.Data;

public static class IdentityOptionsProvider
{
    public static void GetIdentityOptions(this IdentityOptions options)
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.SignIn.RequireConfirmedEmail = true;
    }
}
