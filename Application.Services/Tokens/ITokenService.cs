using Application.Data.Models.Users;
using Application.Services.Models.Users;

namespace Application.Services.Tokens;

public interface ITokenService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user);

    Task<string> GenerateConfirmEmailTokenAsync(UserRequestModels.ConfirmEmail requestModel);

    Task<string> GenerateForgetPasswordTokenAsync(UserRequestModels.ForgetPassword requestModel);
}
