namespace Library.Reporting.Service.ReportHandlers;

public interface IReportHandler
{
    ReportType ReportType { get; }

    Task<TResponse> ExecuteAsync<TRequest,TResponse>(TRequest request);
}
