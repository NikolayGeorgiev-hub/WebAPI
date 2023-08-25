using Application.Services.Models.Users;

namespace Application.Services.Accounts;

public interface IAccountService
{
    Task RegistrationAsync(UserRequestModels.Registration requestModel);

    Task<string> LoginAsync(UserRequestModels.Login requestModel);

    Task<UserResponseModels.Profile> GetUserProfileAsync(Guid userId);
}
