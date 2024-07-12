using System.Text.Json;
using Google.Protobuf;
using Grpc.Net.Client;
using Library.Reporting.Types;
using Library.Reporting.Types.Protos;
using Microsoft.AspNetCore.Mvc;

namespace Library.API;

public static class Endpoints
{
    public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder
        .MapGet("/Reports/{string:reportName}", GetReportHandler)
        .WithOpenApi();

        return endpointRouteBuilder;
    }

    public static string GetReportHandler([FromRoute]string reportName)
    {
        var channel = new GrpcChannel();
        var service = new ReportingService.ReportingServiceClient(channel);
        if(reportName == "MostBorrowedBooks")
        {
            var requestObject = new MostBorrowedBooksRequest();
        }
        var requestData = await ToByteStringAsync<>();
        var request = new GetReportRequest { Data = };
       await service.GetReportAsync();
    }

    private static async Task<T?> FromByteArrayAsync<T>(ByteString data, CancellationToken cancellationToken)
    {
        if (data is null)
            return default;

        using MemoryStream ms = new(data.ToByteArray());
        return await JsonSerializer.DeserializeAsync<T>(ms, cancellationToken: cancellationToken);
    }

    private static async Task<ByteString?> ToByteStringAsync<T>(T obj, CancellationToken cancellationToken)
    {
        if (obj is null)
            return null;

        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, obj, cancellationToken: cancellationToken);
        return UnsafeByteOperations.UnsafeWrap(stream.ToArray());
    }
}