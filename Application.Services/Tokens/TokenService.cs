using Application.Common.Configurations;
using Application.Common.Exceptions;
using Application.Data.Models.Users;
using Application.Services.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Tokens;

public class TokenService  : ITokenService
{
    private readonly JwtConfiguration jwtConfiguration;
    private readonly UserManager<ApplicationUser> userManager;

    public TokenService(IOptions<JwtConfiguration> options,UserManager<ApplicationUser> userManager)
    {
       jwtConfiguration = options.Value;
        this.userManager = userManager;
    }


    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtConfiguration.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
           new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
           new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        }.ToList();

        IList<string> roles = await this.userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
          this.jwtConfiguration.Issuer,
          this.jwtConfiguration.Audience,
          claims,
          null,
          expires: DateTime.Now.AddDays(1),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateConfirmEmailTokenAsync(UserRequestModels.ConfirmEmail requestModel)
    {
        ApplicationUser user = await this.userManager.FindByEmailAsync(requestModel.Email)
           ?? throw new NotFoundUserException("Not found user");

        string confirmEmailToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
        confirmEmailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmEmailToken));

        return confirmEmailToken;
    }


    public async Task<string> GenerateForgetPasswordTokenAsync(UserRequestModels.ForgetPassword requestModel)
    {
        ApplicationUser user = await this.userManager.FindByEmailAsync(requestModel.Email)
            ?? throw new NotFoundUserException("Not found user");

        string resetPasswordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
        resetPasswordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetPasswordToken));

        return resetPasswordToken;
    }
}
