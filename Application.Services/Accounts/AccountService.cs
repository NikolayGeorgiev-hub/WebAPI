using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Data;
using Application.Data.Models.Users;
using Application.Services.Extensions;
using Application.Services.Models.Users;
using Application.Services.Tokens;
using Application.Services.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Application.Services.Accounts;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext dbContext;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ITokenService tokenService;
    private readonly IValidationService validationService;
    private readonly ILogger<AccountService> logger;

    public AccountService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IValidationService validationService,
        ILogger<AccountService> logger)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.tokenService = tokenService;
        this.validationService = validationService;
        this.logger = logger;
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
        };

        IdentityResult result = await this.userManager.CreateAsync(user, requestModel.Password);
        if (!result.Succeeded)
        {
            this.logger.LogError(IdentityResultExtensions.GetIdentityResultMessages(result));
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

    public async Task ConfirmEmailAsync(UserRequestModels.IdentityToken requestModel)
    {
        this.validationService.Validate(requestModel);

        ApplicationUser? user = await this.userManager.FindByEmailAsync(requestModel.Email)
            ?? throw new NotFoundUserException("Not found user");

        string confirmEmailToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(requestModel.Token));

        bool isValidToken = await this.userManager
            .VerifyUserTokenAsync(user, userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", confirmEmailToken);

        if (!isValidToken)
        {
            this.logger.LogError("Invalid email confirmation token value");
            throw new SecurityTokenException();
        }

        IdentityResult result = await this.userManager.ConfirmEmailAsync(user, confirmEmailToken);
        if (!result.Succeeded)
        {
            this.logger.LogError(IdentityResultExtensions.GetIdentityResultMessages(result));
            throw new SecurityTokenException();
        }
    }

    public async Task ResetPasswordAsync(UserRequestModels.IdentityToken tokenModel, UserRequestModels.ResetPassword requestModel)
    {
        this.validationService.Validate(tokenModel);

        ApplicationUser? user = await this.userManager.FindByEmailAsync(tokenModel.Email)
            ?? throw new NotFoundUserException("Not found user");

        if (!user.EmailConfirmed)
        {
            throw new NotConfirmedEmailException("Pleas confirm our email address");
        }

        string resetPasswordToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(tokenModel.Token));

        bool isValidToken = await this.userManager
           .VerifyUserTokenAsync(user, userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetPasswordToken);

        if (!isValidToken)
        {
            this.logger.LogError("Invalid email confirmation token value");
            throw new SecurityTokenException();
        }

        IdentityResult result = await this.userManager.ResetPasswordAsync(user, resetPasswordToken, requestModel.Password);
        if (!result.Succeeded)
        {
            this.logger.LogError(IdentityResultExtensions.GetIdentityResultMessages(result));
            throw new SecurityTokenException();
        }

    }

    public async Task ChangePasswordAsync(Guid userId, UserRequestModels.ChangePassword requestModel)
    {
        ApplicationUser? user = await this.userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundUserException("Not found user with current id");

        PasswordHasher<ApplicationUser> passwordHasher = new();
        bool isCorrectPassword = userManager.PasswordHasher
            .VerifyHashedPassword(user, user.PasswordHash!, requestModel.Password) == PasswordVerificationResult.Success;

        if (!isCorrectPassword)
        {
            throw new InvalidLoginException("Invalid password");
        }

        IdentityResult result = await this.userManager.ChangePasswordAsync(user, requestModel.Password, requestModel.NewPassword);
        if (!result.Succeeded)
        {
            this.logger.LogError(IdentityResultExtensions.GetIdentityResultMessages(result));
            throw new InvalidLoginException(IdentityResultExtensions.GetIdentityResultMessages(result));
        }
    }

    public async Task<UserResponseModels.Profile> GetUserProfileAsync(Guid userId)
    {
        ApplicationUser? user = await this.userManager.FindByIdAsync(userId.ToString());
        return user is null ? throw new NotFoundUserException("Not found user") : user.ToUserProfile();
    }

}
