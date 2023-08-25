using Application.Data.Models.Users;
using Application.Services.Models.Users;

namespace Application.Services.Extensions;

public static class AutoMapperExtensions
{
    public static UserProfileResponseModel ToUserProfileResponse(this ApplicationUser user)
        => new UserProfileResponseModel(user.FirstName, user.Email!, user.PhoneNumber ?? "n/a");


}
