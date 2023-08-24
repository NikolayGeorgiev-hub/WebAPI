using Application.Common.Configurations;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Common.Middleware;

public class TokenValidatorMiddleware
{
    private readonly RequestDelegate requestDelegate;
    private readonly JwtConfiguration jwtConfiguration;

    public TokenValidatorMiddleware(
        RequestDelegate requestDelegate, 
        IOptions<JwtConfiguration> options)
    {
        this.requestDelegate = requestDelegate;
        this.jwtConfiguration = options.Value;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        IHeaderDictionary headers = httpContext.Request.Headers;
        string authorizationHeader = httpContext!.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            var token = authorizationHeader!.Split(" ");
            var tokenHandler = new JwtSecurityTokenHandler();

            var result = await tokenHandler.ValidateTokenAsync(token[1], new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = this.jwtConfiguration.Issuer,
                ValidAudience = this.jwtConfiguration.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtConfiguration.Key))
            });

            if (!result.IsValid)
            {
                await Console.Out.WriteLineAsync($"Token is {result.IsValid}");
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                return;
            }
        }

        await this.requestDelegate(httpContext);
    }
}
