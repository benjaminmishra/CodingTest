using Library.Reporting.Protos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Library.API.Tests;

public class ReportingApiWebApplcationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public Mock<ReportingService.ReportingServiceClient> MockReportingClient { get; } = new(MockBehavior.Strict);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing gRPC client if registered
            var descriptor = services.SingleOrDefault(d =>d.ServiceType == typeof(ReportingService.ReportingServiceClient));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register the mock
            services.AddSingleton(MockReportingClient.Object);
        });
    }
}