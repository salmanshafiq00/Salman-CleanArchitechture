using CleanArchitechture.Infrastructure.Persistence;
using CleanArchitechture.Web.Extensions;
using Hangfire;
using Serilog;

const string Allow_Origin_Policy = "Allow-Origin-Policy";


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = (context) =>
    {
        var env = context.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        if (env.IsDevelopment())
        {
            context.ProblemDetails.Extensions["RequestId"] = context.HttpContext.TraceIdentifier;
        }
    };
});

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
builder.Services.AddKeyVaultIfConfigured(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}


app.UseHealthChecks("/health");
app.UseCors(Allow_Origin_Policy);
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseStaticFiles();

app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseHangfireDashboard(options: new DashboardOptions
{
    Authorization = [],
    DarkModeEnabled = true,
});

app.UseBackgroundJobs();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

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
