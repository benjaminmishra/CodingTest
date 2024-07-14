using Grpc.Core;
using Library.Reporting.DataAccess;
using Library.Reporting.Protos;
using Library.Reporting.Service.ReportHandlers;

namespace Library.Reporting.Service;

public class ReportingService : Protos.ReportingService.ReportingServiceBase
{
    private readonly LibraryDbContext _dbContext;

    public ReportingService(LibraryDbContext libraryDbContext)
    {
        _dbContext = libraryDbContext;
    }

    public override async Task<GetReportResponse> GetReport(GetReportRequest request, ServerCallContext context)
    {
        switch (request.DataCase)
        {
            case GetReportRequest.DataOneofCase.MostBorrowedBooksRequest:
                {
                    var handler = new MostBorrowedBookReportHandler(_dbContext);
                    var queryResult = await handler.ExecuteAsync(request.MostBorrowedBooksRequest, context.CancellationToken);

                    if (queryResult.IsT1)
                        return new GetReportResponse { Error = queryResult.AsT1 };

                    return new GetReportResponse { MostBorrowedBooksReponse = queryResult.AsT0 };
                }
            case GetReportRequest.DataOneofCase.BookStatusRequest:
                {
                    var handler = new BookStatusReportHandler(_dbContext);
                    var queryResult = await handler.ExecuteAsync(request.BookStatusRequest, context.CancellationToken);

                    if (queryResult.IsT1)
                        return new GetReportResponse { Error = queryResult.AsT1 };

                    return new GetReportResponse { BookStatusResponse = queryResult.AsT0 };
                }
            case GetReportRequest.DataOneofCase.UserBorrowedBooksRequest:
                {
                    var handler = new UserBorrowedBooksReportHandler(_dbContext);
                    var queryResult = await handler.ExecuteAsync(request.UserBorrowedBooksRequest, context.CancellationToken);

                    if (queryResult.IsT1)
                        return new GetReportResponse { Error = queryResult.AsT1 };

                    return new GetReportResponse { UserBorrowedBooksResponse = queryResult.AsT0 };
                }
            case GetReportRequest.DataOneofCase.BookReadRateRequest:
                {
                    var handler = new BookReadRateReportHandler(_dbContext);
                    var queryResult = await handler.ExecuteAsync(request.BookReadRateRequest, context.CancellationToken);

                    if (queryResult.IsT1)
                        return new GetReportResponse { Error = queryResult.AsT1 };

                    return new GetReportResponse { BookReadRateResponse = queryResult.AsT0 };
                }
            case GetReportRequest.DataOneofCase.OtherBooksBorrowedBySameUsersRequest:
                {
                    var handler = new OtherBooksBorrowedBySameUsersReportHandler(_dbContext);
                    var queryResult = await handler.ExecuteAsync(request.OtherBooksBorrowedBySameUsersRequest, context.CancellationToken);

                    if (queryResult.IsT1)
                        return new GetReportResponse { Error = queryResult.AsT1 };

                    return new GetReportResponse { OtherBooksBorrowedBySameUsersReponse = queryResult.AsT0 };
                }
            case GetReportRequest.DataOneofCase.MostActiveBorrowersRequest:
                {
                    var handler = new MostActiveBorrowersReportHandler(_dbContext);
                    var queryResult = await handler.ExecuteAsync(request.MostActiveBorrowersRequest, context.CancellationToken);

                    if (queryResult.IsT1)
                        return new GetReportResponse { Error = queryResult.AsT1 };

                    return new GetReportResponse { MostActiveBorrowerResponse = queryResult.AsT0 };
                }
            case GetReportRequest.DataOneofCase.None:
            default:
                return new GetReportResponse { Error = new() { Message = "Report not implemented" } };
        }
    }
}