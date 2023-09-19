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
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Application.Services.Tokens;
using Microsoft.AspNetCore.Identity;
using Application.Services.Validators;
using Application.Services.Validators.Users;
using Application.Services.Administration;
using Application.Data.Seeding;
using Application.Services.Categories;
using Application.Services.Products;
using Application.Services.Ratings;
using Application.Services.Orders;
using Application.Services.Comments;
using Application.Services.Discounts;
using Hangfire;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;

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

        ConfigureHangfire(builder.Services, builder.Configuration);

        builder.Services.AddMvc(options =>
        {
            options.Filters.Add<ExceptionFilter>();
            options.Filters.Add<ModelStateFilter>();
        })
       .AddFluentValidation(options =>
       {
           options.RegisterValidatorsFromAssemblyContaining<UserRequestModels.Registration>();
           options.LocalizationEnabled = true;
       });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        ConfigureRequestLocalization(app, builder.Configuration);

        using (var serviceScope = app.Services.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();

            new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
        }
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
        app.UseHangfireDashboard();

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
        services.Configure<RequestLocalizationConfigurations>(configuration.GetSection(nameof(RequestLocalizationConfigurations)));
    }

    private static void ConfigureRequestLocalization(IApplicationBuilder app, IConfiguration configuration)
    {
        RequestLocalizationConfigurations requestLocalization = new();
        configuration.GetSection(nameof(RequestLocalizationConfigurations)).Bind(requestLocalization);

        IList<CultureInfo> supportedCultures = new List<CultureInfo>();
        requestLocalization.SupportedCultures.ToList()
            .ForEach(x => supportedCultures.Add(new CultureInfo(x)));

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(requestLocalization.DefaultRequestCulture),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IDiscountService, DiscountService>();
    }

    private static void ConfigureHangfire(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHangfire(options => options
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HanfgireConnection"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }).WithJobExpirationTimeout(TimeSpan.FromDays(10)));

        services.AddHangfireServer();
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
        JwtConfiguration jwtConfiguration = new();
        configuration.GetSection(nameof(JwtConfiguration)).Bind(jwtConfiguration);

        services.Configure<DataProtectionTokenProviderOptions>(options =>
         options.TokenLifespan = TimeSpan.FromHours(5));

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