using Application.Common.Configurations;
using Application.Common.Exceptions;
using Application.Data;
using Application.Data.Models.Users;
using Application.Services.Models.Users;
using Microsoft.AspNetCore.Identity;
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
    private readonly JwtConfiguration jwtConfigurations;

    public AccountService(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IOptions<JwtConfiguration> jwtConfigurations)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.jwtConfigurations = jwtConfigurations.Value;
    }


    public async Task RegistrationAsync(UserRegistrationRequestModel requestModel)
    {
        bool existsEmailAddress = await this.dbContext.Users.AnyAsync(x => x.Email == requestModel.Email);
        if (existsEmailAddress)
        {
            throw new ExistsEmailAddressException("The email address already is taken");
        }

        ApplicationUser user = new()
        {
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

    public async Task<string> LoginAsync(UserLoginRequestModel requestModel)
    {
        ApplicationUser? user = await this.userManager.FindByEmailAsync(requestModel.Email);
        if (user is null)
        {
            throw new InvalidLoginException("Invalid email or password");
        }

        PasswordHasher<ApplicationUser> passwordHasher = new();
        bool isCorrectPassword = userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, requestModel.Password) == PasswordVerificationResult.Success;
        if (!isCorrectPassword)
        {
            throw new InvalidLoginException("Invalid email or password");
        }


        IList<string> roles = await this.userManager.GetRolesAsync(user);
        string token = GenerateToken(user, roles);

        return token;
    }

    public string GenerateToken(ApplicationUser user, ICollection<string> roles)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtConfigurations.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
           new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
           new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        }.ToList();

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
          this.jwtConfigurations.Issuer,
          this.jwtConfigurations.Audience,
          claims,
          null,
          expires: DateTime.Now.AddDays(1),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
