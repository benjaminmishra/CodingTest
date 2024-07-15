using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Library.Reporting.Protos;
using Microsoft.AspNetCore.Mvc;

namespace Library.API;

public static class Endpoints
{
    public static IEndpointRouteBuilder RegisterReportsEndpoints(this WebApplication app)
    {
        var routeGroupBuilder = app.MapGroup("/reports").WithOpenApi();

        routeGroupBuilder
        .MapGet("/most-borrowed-books", GetMostBorrowedBooks);

        routeGroupBuilder
        .MapGet("/book-status/{bookId:guid}", GetBookStatus);

        routeGroupBuilder
        .MapGet("/most-active-book-borrowers", GetMostActiveBookBorrowers);

        routeGroupBuilder
       .MapGet("/user-borrowed-books/{borrowerId:guid}", GetUserBorrowedBooks);

        routeGroupBuilder
       .MapGet("/other-books-borrowed-by-same-users/{bookId:guid}", GetOtherBooksBorrowedBySameUsers);

        routeGroupBuilder
       .MapGet("/book-read-rate/{bookId:guid}", GetBookReadRate);

        return app;
    }

    public static async Task<IResult> GetMostBorrowedBooks(
        [FromQuery] int top,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        var getMostBorrowedBooksRequest = new GetReportRequest
        {
            MostBorrowedBooksRequest = new()
            {
                Count = top
            }
        };

        GetReportResponse? reportResult;
        try
        {
            reportResult = await reportingServiceClient.GetReportAsync(getMostBorrowedBooksRequest,
                cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return Results.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return Results.Ok(reportResult.MostBorrowedBooksReponse.MostBorrowedBooks);
    }

    public static async Task<IResult> GetBookStatus(
        [FromRoute] Guid bookId,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        var getBookStatusRequest = new GetReportRequest
        {
            BookStatusRequest = new()
            {
                BookId = bookId.ToString(),
            }
        };

        GetReportResponse? reportResult;
        try
        {
            reportResult = await reportingServiceClient.GetReportAsync(getBookStatusRequest,
                cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return Results.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return Results.Ok(reportResult.BookStatusResponse);
    }

    public static async Task<IResult> GetMostActiveBookBorrowers(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int count,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        var getBookStatusRequest = new GetReportRequest
        {
            MostActiveBorrowersRequest = new()
            {
                StartDate = Timestamp.FromDateTime(startDate.ToUniversalTime()),
                EndDate = Timestamp.FromDateTime(endDate.ToUniversalTime()),
                Count = count,
            }
        };

        GetReportResponse? reportResult;
        try
        {
            reportResult = await reportingServiceClient.GetReportAsync(getBookStatusRequest,
                cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return Results.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return Results.Ok(reportResult.MostActiveBorrowerResponse);
    }

    public static async Task<IResult> GetUserBorrowedBooks(
        [FromRoute] Guid borrowerId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime? endDate,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        var returnDate = endDate ?? DateTime.Now;

        var getUserBorrowedBooksRequest = new GetReportRequest
        {
            UserBorrowedBooksRequest = new()
            {
                BorrowerId = borrowerId.ToString(),
                StartDate = Timestamp.FromDateTime(startDate.ToUniversalTime()),
                EndDate = Timestamp.FromDateTime(returnDate.ToUniversalTime()),
            }
        };

        GetReportResponse? reportResult;
        try
        {
            reportResult = await reportingServiceClient.GetReportAsync(getUserBorrowedBooksRequest,
                cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return Results.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return Results.Ok(reportResult.UserBorrowedBooksResponse);
    }

    public static async Task<IResult> GetOtherBooksBorrowedBySameUsers(
        [FromRoute] Guid bookId,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        var getOtherBooksBorrowedBySameUsersRequest = new GetReportRequest
        {
            OtherBooksBorrowedBySameUsersRequest = new()
            {
                BookId = bookId.ToString(),
            }
        };

        GetReportResponse? reportResult;
        try
        {
            reportResult = await reportingServiceClient.GetReportAsync(getOtherBooksBorrowedBySameUsersRequest,
                cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return Results.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return Results.Ok(reportResult.OtherBooksBorrowedBySameUsersReponse);
    }

    public static async Task<IResult> GetBookReadRate(
        [FromRoute] Guid bookId,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        var getBookReadRateRequest = new GetReportRequest
        {
            BookReadRateRequest = new()
            {
                BookId = bookId.ToString(),
            }
        };

        GetReportResponse? reportResult;
        try
        {
            reportResult = await reportingServiceClient.GetReportAsync(getBookReadRateRequest,
                cancellationToken: cancellationToken);
        }
        catch (RpcException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return Results.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return Results.Ok(reportResult.BookReadRateResponse);
    }
}