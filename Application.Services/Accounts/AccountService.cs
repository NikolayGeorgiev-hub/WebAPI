using Application.Common.Configurations;
using Application.Common.Exceptions;
using Application.Data;
using Application.Data.Models.Users;
using Application.Services.Extensions;
using Application.Services.Models.Users;
using Application.Services.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext dbContext;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ITokenService tokenService;

    public AccountService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.tokenService = tokenService;
    }


    public async Task RegistrationAsync(UserRequestModels.Registration requestModel)
    {
        bool existsEmailAddress = await this.dbContext.Users.AnyAsync(x => x.Email == requestModel.Email);
        if (existsEmailAddress)
        {
            throw new ExistsEmailAddressException("The email address already is taken");
        }

        ApplicationUser user = new()
        {
            FirstName = requestModel.FirstName,
            Email = requestModel.Email,
            UserName = requestModel.Email,
            EmailConfirmed = true
        };

        IdentityResult result = await this.userManager.CreateAsync(user, requestModel.Password);
        if (!result.Succeeded)
        {
            //throw something
        }
    }

    public async Task<string> LoginAsync(UserRequestModels.Login requestModel)
    {
        ApplicationUser? user = await this.userManager.FindByEmailAsync(requestModel.Email)
            ?? throw new InvalidLoginException("Invalid email or password");

        PasswordHasher<ApplicationUser> passwordHasher = new();
        bool isCorrectPassword = userManager.PasswordHasher
            .VerifyHashedPassword(user, user.PasswordHash!, requestModel.Password) == PasswordVerificationResult.Success;

        if (!isCorrectPassword)
        {
            throw new InvalidLoginException("Invalid email or password");
        }

        if (!user.EmailConfirmed)
        {
            throw new NotConfirmedEmailException("Pleas confirm our email address");
        }

        string token = await this.tokenService.GenerateJwtTokenAsync(user);

        return token;
    }

    public async Task ConfirmEmailAsync(UserRequestModels.ConfirmUserEmail requestModel)
    {
        ApplicationUser? user = await this.userManager.FindByEmailAsync(requestModel.Email)
            ?? throw new NotFoundUserException("Not found user");

        string confirmEmailToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(requestModel.Token));

        bool isValidToken = await this.userManager
            .VerifyUserTokenAsync(user, userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", confirmEmailToken);

        if (!isValidToken)
        {
            throw new SecurityTokenException();
        }

        IdentityResult result = await this.userManager.ConfirmEmailAsync(user, confirmEmailToken);
        if (!result.Succeeded)
        {
            throw new SecurityTokenException();
        }
    }

    public async Task ResetPasswordAsync(UserRequestModels.ResetPassword requestModel)
    {
        ApplicationUser? user = await this.userManager.FindByEmailAsync(requestModel.Email)
            ?? throw new NotFoundUserException("Not found user");

        if (!user.EmailConfirmed)
        {
            throw new NotConfirmedEmailException("Pleas confirm our email address");
        }

        string resetPasswordToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(requestModel.Token));

        bool isValidToken = await this.userManager
            .VerifyUserTokenAsync(user, userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetPasswordToken);

        if (!isValidToken)
        {
            throw new SecurityTokenException();
        }

        IdentityResult result = await this.userManager.ResetPasswordAsync(user, resetPasswordToken, requestModel.Password);
        if (!result.Succeeded)
        {
            throw new SecurityTokenException();
        }

    }

    public async Task<UserResponseModels.Profile> GetUserProfileAsync(Guid userId)
    {
        ApplicationUser? user = await this.userManager.FindByIdAsync(userId.ToString());
        return user is null ? throw new NotFoundUserException("Not found user") : user.ToUserProfile();
    }

}
