using Grpc.Core;
using Library.Reporting.Protos;
using Moq;

namespace Library.API.Tests;

public static class MockExtensions
{
    public static void SetupGrpc(this Mock<ReportingService.ReportingServiceClient> mock, GetReportResponse response)
    {
        mock.Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetReportResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => new Status(StatusCode.OK, string.Empty),
                () => [],
                () => { }
            ));
    }

    public static void SetupError(this Mock<ReportingService.ReportingServiceClient> mock)
    {
        mock.Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Throws(new RpcException(new Status(StatusCode.Internal, "Error")));
    }
}
