using System.Net;
using System.Text.Json;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Library.Reporting.Types;
using Library.Reporting.Types.Protos;
using Microsoft.AspNetCore.Mvc;

namespace Library.API;

public static class Endpoints
{
    public static IEndpointRouteBuilder RegisterEndpoints(this WebApplication app)
    {
        app
        .MapGet("/Reports/MostBorrowedBooks/{count}", GetMostBorrowedBooks)
        .WithOpenApi();

        return app;
    }

    public static async Task<IResult> GetMostBorrowedBooks([FromRoute] int count, [FromServices] ReportingService.ReportingServiceClient reportingServiceClient, CancellationToken cancellationToken)
    {
        var getMostBorrowedBooksRequest = new GetReportRequest
        {
            MostBorrowedBooksRequest = new()
            {
                Count = count
            }
        };

        var reportResult = await reportingServiceClient.GetReportAsync(getMostBorrowedBooksRequest, cancellationToken : cancellationToken);

        if (reportResult.Error is not null)
            return Results.Problem(detail: reportResult.Error.Message);

        return Results.Ok(reportResult.MostBorrowedBooksReponse.MostBorrowedBooks);
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