using Library.Reporting.DataAccess;
using Library.Reporting.Service;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// Add services to the container.
builder.Services.Configure<ReportingOptions>(builder.Configuration.GetSection("Reporting"));
builder.Services.AddGrpc();
builder.Services.AddDbContext<LibraryDbContext>((serviceProvider, options) =>
{
    var reportingOptions = serviceProvider.GetRequiredService<IOptions<ReportingOptions>>().Value;
    options.UseSqlServer(reportingOptions.LibraryDbConnStr);
});

var app = builder.Build();

// TODO: Move apply migrations function to its own container
await ApplyMigrations(app);

// Configure the HTTP request pipeline.
app.MapGrpcService<ReportingService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();

static async Task ApplyMigrations(WebApplication app)
{
    // Apply migrations at startup, the database needs to be running at this stage
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<LibraryDbContext>();
            await context.Database.MigrateAsync();
            DataSeeder.SeedBooksData(context);
        }
        catch (Exception ex)
        {
            // Log the error
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}