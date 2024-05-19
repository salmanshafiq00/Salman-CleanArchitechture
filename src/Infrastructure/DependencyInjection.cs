using CleanArchitechture.Application.Common.Caching;
using CleanArchitechture.Application.Common.DapperQueries;
using CleanArchitechture.Domain.Constants;
using CleanArchitechture.Infrastructure.Caching;
using CleanArchitechture.Infrastructure.Persistence;
using CleanArchitechture.Infrastructure.Persistence.Interceptors;
using CleanArchitechture.Infrastructure.Persistence.Services;
using CleanArchitechture.Infrastructure.Identity;
using CleanArchitechture.Infrastructure.OptionsSetup.Jwt;
using CleanArchitechture.Infrastructure.Services.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using WebApi.Infrastructure.Persistence;
using WebApi.Infrastructure.Permissions;
using StackExchange.Redis;
using CleanArchitechture.Application.Common.Abstractions.Identity;
using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Infrastructure.Persistence.Outbox;
using Hangfire;
using CleanArchitechture.Infrastructure.BackgroundJobs;
using Hangfire.SqlServer;
using CleanArchitechture.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConString = configuration.GetConnectionString("DefaultConnection");
        var redisConString = configuration.GetConnectionString("RedisCache");

        Guard.Against.Null(dbConString, message: "Connection string 'DefaultConnection' not found.");
        Guard.Against.Null(redisConString, message: "Connection string 'RedisCache' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, InsertOutboxMessagesInterceptor>();

        services.AddScoped<ISqlConnectionFactory>(_ => new SqlConnectionFactory(dbConString));

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(dbConString);
        });

        services.AddScoped<IApplicationDbContext>(provider
            => provider.GetRequiredService<ApplicationDbContext>());

        services.AddSingleton(ConnectionMultiplexer.Connect(redisConString));
        services.AddStackExchangeRedisCache(options => options.Configuration = redisConString);

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddScoped<ICommonQueryService, CommonQueryService>();

        services.AddScoped<IDateTimeProvider, DateTimeService>();

        // Hangfire
        services.AddHangfire(options =>
        {
            options.UseSqlServerStorage(dbConString, new SqlServerStorageOptions
            {
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            });
        });

        services.AddHangfireServer();

        services.AddScoped<ProcessOutboxMessagesJob>();

        // Adding Caching
        services.AddDistributedMemoryCache();
        services.ConfigureOptions<CacheOptionsSetup>();
        services.AddSingleton<IInMemoryCacheService, InMemoryCacheService>();
        services.AddSingleton<IDistributedCacheService, DistributedCacheService>();

        services.AddTransient<IIdentityService, IdentityService>();
        //services.AddTransient<IIdentityRoleService, IdentityRoleService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IAccessTokenProvider, AccessTokenProvider>();
        services.AddTransient<IRefreshTokenProvider, RefreshTokenProvider>();
        services.AddTransient<ITokenProviderService, TokenProviderService>();


        //services.AddAuthentication()
        //    .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer();

        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));

        });

        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        // For dynamically create policy if not exist
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        return services;
    }
}
