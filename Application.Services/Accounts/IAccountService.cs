using Application.Services.Models.Users;

namespace Application.Services.Accounts;

public interface IAccountService
{
    Task RegistrationAsync(UserRegistrationRequestModel requestModel);

    Task<string> LoginAsync(UserLoginRequestModel requestModel);

    Task<UserProfileResponseModel> GetUserProfileAsync(Guid userId);
}
