using Library.Reporting.Service;
using Library.Reporting.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ReportingOptions>(builder.Configuration.GetSection("Reporting"));
builder.Services.AddGrpc();
builder.Services.AddDbContext<LibraryDbContext>((serviceProvider, options) =>
{
    var reportingOptions = serviceProvider.GetRequiredService<IOptions<ReportingOptions>>().Value;
    options.UseSqlServer(reportingOptions.LibraryDbConnStr);
});

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LibraryDbContext>();
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        // Log the error
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
//
app.MapGrpcService<Library.Reporting.Service.ReportingService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();
