using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Library.Reporting.Protos;
using Microsoft.AspNetCore.Http.HttpResults;
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

    public static async Task<Results<Ok<MostBorrowedBook[]>, ProblemHttpResult, BadRequest>> GetMostBorrowedBooks(
        [FromQuery] int top,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        if (top <= 0)
            return TypedResults.BadRequest();

        GetReportRequest getMostBorrowedBooksRequest = new()
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
            return TypedResults.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return TypedResults.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return TypedResults.Ok(reportResult.MostBorrowedBooksReponse.MostBorrowedBooks.ToArray());
    }

    public static async Task<Results<Ok<BookStatusResponse>, ProblemHttpResult>> GetBookStatus(
        [FromRoute] Guid bookId,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        GetReportRequest getBookStatusRequest = new()
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
            return TypedResults.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return TypedResults.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return TypedResults.Ok(reportResult.BookStatusResponse);
    }

    public static async Task<Results<Ok<MostActiveBorrower[]>, ProblemHttpResult, BadRequest<string>>> GetMostActiveBookBorrowers(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int count,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        var startDt = Timestamp.FromDateTime(startDate.ToUniversalTime());
        var endDt = Timestamp.FromDateTime(endDate.ToUniversalTime());

        if (startDate > endDate)
            return TypedResults.BadRequest("StartDate cannot be greater than end date");

        if (count < 0)
            return TypedResults.BadRequest("Count cannot be negetive");

        GetReportRequest getBookStatusRequest = new()
        {
            MostActiveBorrowersRequest = new()
            {
                StartDate = startDt,
                EndDate = endDt,
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
            return TypedResults.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return TypedResults.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return TypedResults.Ok(reportResult.MostActiveBorrowerResponse.Borrowers.ToArray());
    }

    public static async Task<Results<Ok<BorrowedBookInfo[]>, ProblemHttpResult>> GetUserBorrowedBooks(
        [FromRoute] Guid borrowerId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime? endDate,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        var returnDate = endDate ?? DateTime.Now;

        GetReportRequest getUserBorrowedBooksRequest = new()
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
            return TypedResults.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return TypedResults.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return TypedResults.Ok(reportResult.UserBorrowedBooksResponse.Books.ToArray());
    }

    public static async Task<Results<Ok<BorrowedBookInfo[]>, ProblemHttpResult>> GetOtherBooksBorrowedBySameUsers(
        [FromRoute] Guid bookId,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        GetReportRequest getOtherBooksBorrowedBySameUsersRequest = new()
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
            return TypedResults.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return TypedResults.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return TypedResults.Ok(reportResult.OtherBooksBorrowedBySameUsersReponse.Books.ToArray());
    }

    public static async Task<Results<Ok<BookReadRateResponse>, ProblemHttpResult>> GetBookReadRate(
        [FromRoute] Guid bookId,
        [FromServices] ReportingService.ReportingServiceClient reportingServiceClient,
        CancellationToken cancellationToken)
    {
        GetReportRequest getBookReadRateRequest = new ()
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
            return TypedResults.Problem(detail: ex.Message, statusCode: 500);
        }

        if (reportResult.Error is not null)
            return TypedResults.Problem(detail: reportResult.Error.Message, statusCode: 400);

        return TypedResults.Ok(reportResult.BookReadRateResponse);
    }
}