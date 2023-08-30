using Application.Common;
using Application.Common.Extensions;
using Application.Services.Accounts;
using Application.Services.Models.Users;
using Application.Services.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService accountService;
    private readonly ITokenService tokenService;

    public AccountController(
        IAccountService accountService,
        ITokenService tokenService)
    {
        this.accountService = accountService;
        this.tokenService = tokenService;
    }

    [HttpPost("registration")]
    public async Task<ResponseContent> RegistrationAsync([FromBody] UserRequestModels.Registration requestModel)
    {
        await this.accountService.RegistrationAsync(requestModel);
        return new ResponseContent();
    }

    [HttpPost("login")]
    public async Task<ResponseContent<string>> LoginAsync([FromBody] UserRequestModels.Login requestModel)
    {
        string token = await accountService.LoginAsync(requestModel);
        return new ResponseContent<string>()
        {
            Result = token
        };
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ResponseContent<UserResponseModels.Profile>> GetUserProfileAsync()
    {
        UserResponseModels.Profile user = await this.accountService.GetUserProfileAsync(this.User.GetUserId());
        return new ResponseContent<UserResponseModels.Profile>()
        {
            Result = user
        };
    }

    [HttpPost("forget-password")]
    public async Task<ResponseContent<string>> GetForgetPasswordTokenAsync(UserRequestModels.ForgetPassword requestModel)
    {
        string resetPasswordToken = await this.tokenService.GenerateForgetPasswordTokenAsync(requestModel);

        string? callBack = Url.Action(
            action: "ResetPassword",
            controller: "Account",
            new
            {
                token = resetPasswordToken,
                email = requestModel.Email
            }, Request.Scheme);

        return new ResponseContent<string>
        {
            Result = callBack
        };
    }

    [HttpPost("confirm-email")]
    public async Task<ResponseContent<string>> GetConfirmEmailTokenAsync(UserRequestModels.ConfirmEmail requestModel)
    {
        string confirmEmailToken = await this.tokenService.GenerateConfirmEmailTokenAsync(requestModel);

        string? callBack = Url.Action(
            action: "ConfirmEmail",
            controller: "Account",
            new
            {
                token = confirmEmailToken,
                email = requestModel.Email
            }, Request.Scheme);

        return new ResponseContent<string>
        {
            Result = callBack
        };
    }

    [HttpPost("confirm")]
    public async Task<ResponseContent> ConfirmEmailAsync([FromQuery] string token, [FromQuery] string email)
    {
        UserRequestModels.IdentityToken requestModel = new(token, email);

        await this.accountService.ConfirmEmailAsync(requestModel);
        return new ResponseContent();
    }

    [HttpPost("reset-password")]
    public async Task<ResponseContent> ResetPasswordAsync([FromQuery] string token, [FromQuery] string email, UserRequestModels.ResetPassword requestModel)
    {
        UserRequestModels.IdentityToken tokenModel = new(token, email);

        await this.accountService.ResetPasswordAsync(tokenModel, requestModel);

        return new ResponseContent();
    }
}
