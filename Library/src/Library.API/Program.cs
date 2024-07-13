using Library.API;
using Library.Reporting.Types.Protos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<ReportingService.ReportingServiceClient>(x =>
{
    var reportingServiceAddress = builder.Configuration.GetValue<string>("Api:ReportingServiceUrl") ?? throw new MissingFieldException();
    x.Address = new Uri(reportingServiceAddress);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterEndpoints();

await app.RunAsync();
