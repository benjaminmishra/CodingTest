using Library.Reporting.Services;
using Library.ReportingService;
using Library.ReportingService.DataAccess;
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

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();
