using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Infrastructure.Persistence;
using CleanArchitechture.Web.Extensions;
using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

const string Allow_Origin_Policy = "Allow-Origin-Policy";

var builder = WebApplication.CreateBuilder(args);

// Configure Services

builder.Services.AddCors(options =>
{
    options.AddPolicy(Allow_Origin_Policy, builder =>
    {
        builder.WithOrigins("http://localhost:4200", "http://localhost:8114", "http://localhost:8081", "http://localhost:4444")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Host
    .UseSerilog((context, loggerContext)
        => loggerContext.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();


// Configure the HTTP request pipeline.

var app = builder.Build();

// Set the service provider
ServiceLocator.ServiceProvider = app.Services;

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();

    app.UseSwaggerUi(settings =>
    {
        settings.Path = "/api";
        settings.DocumentPath = "/api/specification.json";
    });
}
else
{
    app.UseHsts();
}

app.UseCors(Allow_Origin_Policy);
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseStaticFiles();

app.UseHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseHangfireDashboard(options: new DashboardOptions
{
    Authorization = [],
    DarkModeEnabled = true,
});

app.UseBackgroundJobs();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/api"));

app.MapEndpoints();

app.Run();

public partial class Program { }
