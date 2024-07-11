using Library.Reporting.DataAccess;

namespace Library.Reporting.Service.ReportHandlers;

public class ReportHandlerFactory : IReportHandlerFactory
{
    private readonly LibraryDbContext _libraryDbContext;

    public ReportHandlerFactory(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public IReportHandler CreateHandler(ReportType type)
    {
        return type switch
        {
            ReportType.MostBorrowedBooks => new MostBorrowedBookReportHandler(_libraryDbContext),
            _ => throw new NotImplementedException()
        };
    }
}
