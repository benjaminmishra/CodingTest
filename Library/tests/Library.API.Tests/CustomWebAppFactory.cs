using System.Data.Common;
using Library.Reporting.Protos;
using Library.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Library.API.Tests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(WebApplicationBuilder builder)
    {
        var mockReportingClient = new Mock<ReportingService.ReportingServiceClient>();
        builder.Services.AddSingleton(mockReportingClient.Object);
        
        var app = builder.Build();
        app.RegisterReportsEndpoints();
    }
}