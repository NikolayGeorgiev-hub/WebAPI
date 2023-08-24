using Application.Common.Configurations;
using Application.Data;
using Application.Data.Models.Users;
using Application.Services.Accounts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FluentValidation.AspNetCore;

using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Services.Models.Users;
using Application.Common.Filters;
using Application.Common.Middleware;

internal class Program
{
    [Obsolete]
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureAppSettings(builder.Host, builder.Environment);
        AddApplicationConfigurations(builder.Services, builder.Configuration);

        AddApplicationServices(builder.Services);
        builder.Services.AddControllers();

        ConfigureApplicationContext(builder.Services, builder.Configuration);
        ConfigureJwtToken(builder.Services, builder.Configuration);


        builder.Services.AddMvc(options =>
        {
            options.Filters.Add<ExceptionFilter>();
        })
       .AddFluentValidation(options =>
       {
           options.RegisterValidatorsFromAssemblyContaining<UserRegistrationRequestModel>();
           options.LocalizationEnabled = true;
       });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<TokenValidatorMiddleware>();
        app.MapControllers();

        app.Run();
    }
    private static void ConfigureAppSettings(IHostBuilder hostBuilder, IHostEnvironment environment)
    {
        hostBuilder.ConfigureAppConfiguration(config =>
        {
            config
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
        });
    }

    private static void AddApplicationConfigurations(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfiguration>(configuration.GetSection(nameof(JwtConfiguration)));
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
    }

    private static void ConfigureApplicationContext(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        //Throw exception if connection string is null

        services.AddDbContext<ApplicationDbContext>(
           options => options.UseSqlServer(connectionString));

        services
            .AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }

    private static void ConfigureJwtToken(IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = new JwtConfiguration();
        configuration.GetSection(nameof(JwtConfiguration)).Bind(jwtConfiguration);


        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfiguration.Issuer,
                ValidAudience = jwtConfiguration.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Key))
            };
        });

    }
}