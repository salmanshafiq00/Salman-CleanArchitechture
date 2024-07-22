using System.Security.Claims;
using CleanArchitechture.Application.Common.Security;
using CleanArchitechture.Domain.Constants;
using CleanArchitechture.Infrastructure.Identity;
using CleanArchitechture.Infrastructure.Identity.Permissions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitechture.Infrastructure.Persistence;

public static class IdentityInitialiserExtensions
{
    public static async Task IdentityInitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var identityInitialiser = scope.ServiceProvider.GetRequiredService<IdentityDbContextInitialiser>();

        await identityInitialiser.InitialiseAsync();

        await identityInitialiser.SeedAsync();
    }
}

internal sealed class IdentityDbContextInitialiser(
        ILogger<IdentityDbContextInitialiser> logger, 
        IdentityContext context, 
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager)
{ 
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedDefaultIdentityAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedDefaultIdentityAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }

        // Get Permission
        var features = Permissions.GetAllNestedModule(typeof(Permissions.Admin));
        features.AddRange(Permissions.GetAllNestedModule(typeof(Permissions.CommonSetup)));

        var permissions = Permissions.GetPermissionsByfeatures(features);

        // Default Permissions
        foreach (var permission in permissions)
        {
            await roleManager.AddClaimAsync(administratorRole, new Claim(CustomClaimTypes.Permission, permission));
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await userManager.CreateAsync(administrator, "Salman@123");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await userManager.AddToRolesAsync(administrator, [administratorRole.Name]);
            }
        }
    }
}
