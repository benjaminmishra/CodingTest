using System.Text.Json;
using Google.Protobuf;
using Grpc.Core;
using Library.Reporting.DataAccess;
using Library.Reporting.Service.ReportHandlers;
using Library.Reporting.Types;
using Library.Reporting.Types.Protos;
using MediatR;
using Protos = Library.Reporting.Types.Protos;

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
                var handler = new MostBorrowedBookReportHandler(_dbContext);
                var queryResult = await handler.ExecuteAsync(request.MostBorrowedBooksRequest, context.CancellationToken);
                return new GetReportResponse { MostBorrowedBooksReponse = queryResult };
            default:
                throw new NotImplementedException();
        }
    }
}