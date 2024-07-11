using Azure.Core;

namespace Library.Reporting.Service.ReportHandlers;

public interface IReportHandlerFactory
{
    IReportHandler CreateHandler(ReportType type);
}
