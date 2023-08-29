using Application.Data.Models.Users;
using Application.Services.Models.Users;

namespace Application.Services.Accounts;

public interface IAccountService
{
    Task RegistrationAsync(UserRequestModels.Registration requestModel);

    Task<string> LoginAsync(UserRequestModels.Login requestModel);

    Task ConfirmEmailAsync(UserRequestModels.ConfirmUserEmail requestModel);

    Task ResetPasswordAsync(UserRequestModels.ResetPassword requestModel);

    Task<UserResponseModels.Profile> GetUserProfileAsync(Guid userId);
}


