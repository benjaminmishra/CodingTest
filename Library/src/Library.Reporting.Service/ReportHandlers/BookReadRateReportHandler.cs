using Library.Reporting.DataAccess;
using Library.Reporting.Protos;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Error = Library.Reporting.Protos.Error;

namespace Library.Reporting.Service.ReportHandlers;

public class BookReadRateReportHandler
{
    private readonly LibraryDbContext _libraryDbContext;

    public BookReadRateReportHandler(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<OneOf<BookReadRateResponse, Error>> ExecuteAsync(BookReadRateRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.BookId, out var bookGuid))
            return new Error { Message = "BookId is an invalid Guid" };

        var readRates = await _libraryDbContext.BorrowedBooks
            .Where(bb => bb.BookId == bookGuid && bb.ReturnedDate != null)
            .Select(bb => new
            {
                bb.BorrowedDate,
                bb.ReturnedDate,
                DaysBorrowed = (bb.ReturnedDate!.Value - bb.BorrowedDate).TotalDays,
                bb.Book.Pages
            })
            .ToListAsync(cancellationToken);

        if (readRates.Count == 0)
            return new Error { Message = "No completed borrow records found for the specified book" };

        var averageReadRate = readRates.Average(rr => rr.Pages / rr.DaysBorrowed);

        return new BookReadRateResponse { AverageReadRate = Math.Round(averageReadRate, 2) };
    }
}
