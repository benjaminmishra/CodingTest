using Library.API;
using Library.Reporting.Protos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpcClient<ReportingService.ReportingServiceClient>(x =>
{
    var reportingServiceAddress = builder.Configuration.GetValue<string>("Api:ReportingServiceUrl")
                                  ?? throw new MissingFieldException("ReportingServiceUrl not set as an configuration item");

    x.Address = new Uri(reportingServiceAddress);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.RegisterReportsEndpoints();

await app.RunAsync();
