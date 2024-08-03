using CleanArchitechture.Application.Common.Abstractions;
using CleanArchitechture.Infrastructure.Communications;
using CleanArchitechture.Web.Middlewares;
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
        builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
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
    //await app.IdentityInitialiseDatabaseAsync();
    //await app.AppInitialiseDatabaseAsync();

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(Allow_Origin_Policy);
app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseAuthentication();
app.Use(async (context, next) =>
{
    var contexta = context;
    await next.Invoke();
});
app.UseRouting();
app.UseAuthorization();
app.UseSerilogRequestLogging();

app.UseHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


app.UseHangfireDashboard(options: new DashboardOptions
{
    Authorization = [],
    DarkModeEnabled = true,
});

app.UseBackgroundJobs();

//app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.UseExceptionHandler(options => { });

app.MapHub<NotificationHub>("/notificationHub");

app.Map("/", () => Results.Redirect("/api"));

app.MapEndpoints();

app.Run();

public partial class Program { }
