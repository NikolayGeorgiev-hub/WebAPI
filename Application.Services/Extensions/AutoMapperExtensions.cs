using Application.Data.Models.Users;
using Application.Services.Models.Users;

namespace Application.Services.Extensions;

public static class AutoMapperExtensions
{
    public static UserResponseModels.Profile ToUserProfile(this ApplicationUser user)
        => new UserResponseModels.Profile(user.FirstName, user.Email!, user.PhoneNumber ?? "n/a");
}
