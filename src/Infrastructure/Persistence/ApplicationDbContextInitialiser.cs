using CleanArchitechture.Domain.Admin;
using CleanArchitechture.Domain.Common;
using CleanArchitechture.Domain.Constants;
using CleanArchitechture.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitechture.Infrastructure.Persistence;


public static class ApplicationInitialiserExtensions
{
    public static async Task AppInitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var appInitialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await appInitialiser.InitialiseAsync();

        await appInitialiser.SeedAsync();
    }
}

internal sealed class ApplicationDbContextInitialiser(
    ApplicationDbContext appDbContext,
    IdentityContext identityContext,
    ILogger<ApplicationDbContextInitialiser> logger)
{

    public async Task InitialiseAsync()
    {
        try
        {
            await appDbContext.Database.MigrateAsync();
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
            await SeedDefaultDataAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedDefaultDataAsync()
    {
        Lookup menuTypeLookup = new() { Name = "Menu Type", Code = "menu", DevCode = 101, Status = true };
        
        await appDbContext.Lookups.AddAsync(menuTypeLookup);

        List<LookupDetail> lookupDetails = [
            new() {Name = "Module", Code = "module", LookupId = menuTypeLookup.Id, DevCode = 10101, Status = true},
            new() {Name = "Sub Menu", Code = "smenu", LookupId = menuTypeLookup.Id, DevCode = 10102, Status = true},
            new() {Name = "Menu", Code = "menu", LookupId = menuTypeLookup.Id, DevCode = 10103, Status = true}
        ];

        await appDbContext.LookupDetails.AddRangeAsync(lookupDetails);


        var moduleId = lookupDetails.FirstOrDefault(x => x.DevCode == 10101)?.Id;
        var menuId = lookupDetails.FirstOrDefault(x => x.DevCode == 10103)?.Id;

        List<AppMenu> appmunes = [
            new () {Label = "Dashboard", RouterLink = "/", ParentId = null,  IsActive = true, OrderNo = 0, Visible = true, MenuTypeId = moduleId.Value},    
            new () {Label = "Admin", RouterLink = "/admin", ParentId = null,  IsActive = true, OrderNo = 1, Visible = true, MenuTypeId = moduleId.Value},   
            new () {Label = "Users", RouterLink = "/admin/users", ParentId = null,  IsActive = true, OrderNo = 1, Visible = true, MenuTypeId = menuId.Value},    
            new () {Label = "Roles", RouterLink = "/admin/roles", ParentId = null,  IsActive = true, OrderNo = 2, Visible = true, MenuTypeId = menuId.Value},    
            new () {Label = "App Menu", RouterLink = "/admin/app-menus", ParentId = null,  IsActive = true, OrderNo = 3, Visible = true, MenuTypeId = menuId.Value},    
            new () {Label = "App Page", RouterLink = "/admin/app-pages", ParentId = null,  IsActive = true, OrderNo = 4, Visible = true, MenuTypeId = menuId.Value},    
            new () {Label = "Common Setup", RouterLink = "/setup", ParentId = null,  IsActive = true, OrderNo = 2, Visible = true, MenuTypeId = moduleId.Value},    
            new () {Label = "Lookup", RouterLink = "/setup/lookups", ParentId = null,  IsActive = true, OrderNo = 1, Visible = true, MenuTypeId = menuId.Value},    
            new () {Label = "Lookp Detail", RouterLink = "/setup/lookup-details", ParentId = null,  IsActive = true, OrderNo = 2, Visible = true, MenuTypeId = menuId.Value}    
        ];

        appDbContext.AddRange(appmunes);

        List<AppPage> apppages = [
            new (){Id = Guid.Parse("4007d345-6ede-43f3-3f91-08dca9b2d959"), Title = "App Users", ComponentName = "UserListComponent", AppPageLayout = "{\"appPageFields\":[{\"id\":\"fld_ec74e\",\"fieldName\":\"username\",\"caption\":\"Username\",\"fieldType\":\"string\",\"dbField\":\"u.Username\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":1,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_5c446\",\"fieldName\":\"email\",\"caption\":\"Email\",\"fieldType\":\"string\",\"dbField\":\"u.Email\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":2,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_63147\",\"fieldName\":\"firstName\",\"caption\":\"First Name\",\"fieldType\":\"string\",\"dbField\":\"u.FirstName\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":3,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_95849\",\"fieldName\":\"lastName\",\"caption\":\"Last Name\",\"fieldType\":\"string\",\"dbField\":\"u.LastName\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":4,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_93f4d\",\"fieldName\":\"phoneNumber\",\"caption\":\"Phone Number\",\"fieldType\":\"string\",\"dbField\":\"u.PhoneNumber\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":5,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_a364b\",\"fieldName\":\"status\",\"caption\":\"Status\",\"fieldType\":\"select\",\"dbField\":\"u.IsActive\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":false,\"filterType\":null,\"dSName\":\"statusSelectList\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":6,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_d254f\",\"fieldName\":\"assignedRoles\",\"caption\":\"Roles\",\"fieldType\":\"multiselect\",\"dbField\":\"ur.RoleId\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":false,\"filterType\":null,\"dSName\":\"roleSelectList\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":7,\"isActive\":true,\"showProperties\":true}]}"},
            new (){Id = Guid.Parse("9a08e9a6-c641-44b1-3f92-08dca9b2d959"), Title = "App Roles", ComponentName = "RoleListComponent", AppPageLayout = "{\"appPageFields\":[{\"id\":\"fld_ff448\",\"fieldName\":\"name\",\"caption\":\"Name\",\"fieldType\":\"string\",\"dbField\":\"r.Name\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":1,\"isActive\":true,\"showProperties\":true}]}"},
            new (){Id = Guid.Parse("5255d7a0-49b8-45da-3f93-08dca9b2d959"), Title = "App Menus", ComponentName = "AppMenuListComponent", AppPageLayout = "{\"appPageFields\":[{\"id\":\"fld_7bb45\",\"fieldName\":\"label\",\"caption\":\"Label\",\"fieldType\":\"string\",\"dbField\":\"m.Label\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":1,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_2ec47\",\"fieldName\":\"routerLink\",\"caption\":\"RouterLink\",\"fieldType\":\"string\",\"dbField\":\"m.RouterLink\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":2,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_92e40\",\"fieldName\":\"parentName\",\"caption\":\"Parent Name\",\"fieldType\":\"multiselect\",\"dbField\":\"m.ParentId\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":false,\"filterType\":null,\"dSName\":\"parentSelectList\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":3,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_68a40\",\"fieldName\":\"active\",\"caption\":\"Active\",\"fieldType\":\"select\",\"dbField\":\"m.Active\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":false,\"filterType\":null,\"dSName\":\"statusSelectList\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":4,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_0aa46\",\"fieldName\":\"visibility\",\"caption\":\"Visible\",\"fieldType\":\"string\",\"dbField\":\"m.Visible\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":false,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":5,\"isActive\":true,\"showProperties\":true}]}"},
            new (){Id = Guid.Parse("bfeb0511-908d-4b9f-b936-08dca9805463"), Title = "App Pages", ComponentName = "AppPageListComponent", AppPageLayout = "{\n  \"appPageFields\": [\n    {\n      \"id\": \"fld_f714d\",\n      \"fieldName\": \"id\",\n      \"caption\": \"Id\",\n      \"fieldType\": \"string\",\n      \"dbField\": \"ap.Id\",\n      \"format\": \"\",\n      \"textAlign\": \"center\",\n      \"isSortable\": false,\n      \"isFilterable\": false,\n      \"isGlobalFilterable\": true,\n      \"filterType\": null,\n      \"dSName\": \"\",\n      \"enableLink\": false,\n      \"linkBaseUrl\": \"\",\n      \"linkValueFieldName\": \"\",\n      \"bgColor\": \"\",\n      \"color\": \"\",\n      \"isVisible\": true,\n      \"sortOrder\": 1,\n      \"isActive\": true,\n      \"showProperties\": true\n    },\n    {\n      \"id\": \"fld_7a54b\",\n      \"fieldName\": \"componentName\",\n      \"caption\": \"Component Name\",\n      \"fieldType\": \"string\",\n      \"dbField\": \"ap.ComponentName\",\n      \"format\": \"\",\n      \"textAlign\": \"center\",\n      \"isSortable\": true,\n      \"isFilterable\": false,\n      \"isGlobalFilterable\": true,\n      \"filterType\": null,\n      \"dSName\": \"\",\n      \"enableLink\": false,\n      \"linkBaseUrl\": \"\",\n      \"linkValueFieldName\": \"\",\n      \"bgColor\": \"\",\n      \"color\": \"\",\n      \"isVisible\": true,\n      \"sortOrder\": 2,\n      \"isActive\": true,\n      \"showProperties\": true\n    },\n    {\n      \"id\": \"fld_eca48\",\n      \"fieldName\": \"title\",\n      \"caption\": \"Title\",\n      \"fieldType\": \"string\",\n      \"dbField\": \"ap.Title\",\n      \"format\": \"\",\n      \"textAlign\": \"center\",\n      \"isSortable\": true,\n      \"isFilterable\": false,\n      \"isGlobalFilterable\": true,\n      \"filterType\": null,\n      \"dSName\": \"\",\n      \"enableLink\": false,\n      \"linkBaseUrl\": \"\",\n      \"linkValueFieldName\": \"\",\n      \"bgColor\": \"\",\n      \"color\": \"\",\n      \"isVisible\": true,\n      \"sortOrder\": 3,\n      \"isActive\": true,\n      \"showProperties\": true\n    }\n  ]\n}"},
            new (){Id = Guid.Parse("1bea0afd-ebcc-491f-3424-08dca9afe0ca"), Title = "Lookup List", ComponentName = "LookupListComponent", AppPageLayout = "{\"appPageFields\":[{\"id\":\"fld_6d24b\",\"fieldName\":\"code\",\"caption\":\"Code\",\"fieldType\":\"string\",\"dbField\":\"l.Code\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":1,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_0fe49\",\"fieldName\":\"name\",\"caption\":\"Name\",\"fieldType\":\"string\",\"dbField\":\"l.Name\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":2,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_6194b\",\"fieldName\":\"parentName\",\"caption\":\"Parent Name\",\"fieldType\":\"multiselect\",\"dbField\":\"l.ParentId\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"parentSelectList\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":2,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_88349\",\"fieldName\":\"description\",\"caption\":\"Description\",\"fieldType\":\"string\",\"dbField\":\"l.Description\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":false,\"isFilterable\":false,\"isGlobalFilterable\":false,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":4,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_bd341\",\"fieldName\":\"statusName\",\"caption\":\"Status\",\"fieldType\":\"select\",\"dbField\":\"L.Status\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":false,\"filterType\":null,\"dSName\":\"statusSelectList\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":5,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_bd541\",\"fieldName\":\"created\",\"caption\":\"Created\",\"fieldType\":\"date\",\"dbField\":\"l.Created\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":6,\"isActive\":true,\"showProperties\":true}]}"},
            new (){Id = Guid.Parse("687c6b12-763a-47d7-3f90-08dca9b2d959"), Title = "Lookup Detail List", ComponentName = "LookupDetailListComponent", AppPageLayout = "{\"appPageFields\":[{\"id\":\"fld_ad74d\",\"fieldName\":\"name\",\"caption\":\"Name\",\"fieldType\":\"string\",\"dbField\":\"ld.Name\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":2,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_d4542\",\"fieldName\":\"code\",\"caption\":\"Code\",\"fieldType\":\"string\",\"dbField\":\"ld.Code\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":1,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_8b44f\",\"fieldName\":\"parentName\",\"caption\":\"Parent Name\",\"fieldType\":\"multiselect\",\"dbField\":\"p.Name\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":true,\"filterType\":null,\"dSName\":\"parentSelectList\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":3,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_57846\",\"fieldName\":\"description\",\"caption\":\"Description\",\"fieldType\":\"string\",\"dbField\":\"ld.Description\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":false,\"isGlobalFilterable\":false,\"filterType\":null,\"dSName\":\"\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":4,\"isActive\":true,\"showProperties\":false},{\"id\":\"fld_5744d\",\"fieldName\":\"statusName\",\"caption\":\"Status\",\"fieldType\":\"select\",\"dbField\":\"ld.Status\",\"format\":\"\",\"textAlign\":\"center\",\"isSortable\":true,\"isFilterable\":true,\"isGlobalFilterable\":false,\"filterType\":null,\"dSName\":\"statusSelectList\",\"enableLink\":false,\"linkBaseUrl\":\"\",\"linkValueFieldName\":\"\",\"bgColor\":\"\",\"color\":\"\",\"isVisible\":true,\"sortOrder\":5,\"isActive\":true,\"showProperties\":true}]}"},
        ];

        appDbContext.AddRange(apppages);

        await appDbContext.SaveChangesAsync();

        var adminRole = await identityContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == Roles.Administrator);

        appDbContext.RoleMenus.AddRange(appmunes.Select(x => new RoleMenu { RoleId = adminRole.Id, AppMenuId = x.Id }));

        await appDbContext.SaveChangesAsync();
    }
}
