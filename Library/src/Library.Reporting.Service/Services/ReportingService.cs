using System.Text.Json;
using Google.Protobuf;
using Grpc.Core;
using Library.Reporting.Service.ReportHandlers;
using Library.Reporting.Types;
using Library.Reporting.Types.Protos;
using MediatR;
using Protos = Library.Reporting.Types.Protos;

namespace Library.Reporting.Service;

public class ReportingService : Protos.ReportingService.ReportingServiceBase
{
    private readonly IMediator _mediator;

    public ReportingService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetReportResponse> GetReport(GetReportRequest request, ServerCallContext context)
    {
        try
        {
            return request.ReportType switch
            {
                ReportTypes.MostBorrowedBooks => await _mediator.Send(request.MostBorrowedBooksRequest, context.CancellationToken),
                _ => new() { Error = new() { Message = "Report Type not recognized" } },
            };
        }
        catch (Exception ex)
        {
            var status = new Status(StatusCode.Internal, ex.Message);
            throw new RpcException(status);
        }
    }

    private async Task<GetReportResponse> ProcessAsync<TRequest, TResponse>(GetReportRequest request, CancellationToken cancellationToken) 
    where TRequest : IMessage, new()
    where TResponse : IMessage, new()
    {
        var query = await request.DataCase
        if (query is null)
            return new() { Error = new() { Message = "Report Type not recognized" } };

        var result = await _mediator.Send(query, cancellationToken);
        if (result is null)
            return new() { Error = new() { Message = "Failed to process request" } };

        var resultByteString = await ToByteStringAsync((TResponse)result, cancellationToken);
        if (resultByteString is null)
            return new() { Error = new() { Message = "Failed to process response" } };

        return new() { Data = resultByteString };
    }
}