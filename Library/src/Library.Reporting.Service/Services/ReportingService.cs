namespace Library.Reporting.Service;

using System.Threading.Tasks;
using Grpc.Core;
using Library.Reporting.Types.Protos;
using Protos = Types.Protos;

public class ReportingService : Protos.ReportingService.ReportingServiceBase
{
    public override async Task<GetReportResponse> GetReport(GetReportRequest request, ServerCallContext context)
    {
        
    }
}