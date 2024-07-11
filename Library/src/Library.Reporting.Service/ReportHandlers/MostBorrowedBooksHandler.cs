using Library.Reporting.Types;
using Library.Reporting.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Library.Reporting.Service.ReportHandlers;

public class MostBorrowedBookReportHandler : IReportHandler
{
    private readonly LibraryDbContext _libraryDbContext;

    public MostBorrowedBookReportHandler(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public ReportType ReportType => ReportType.MostBorrowedBooks;

    public async Task<MostBorrowedBooksResponse> ExecuteAsync<MostBorrowedBooksRequest,MostBorrowedBooksResponse>(MostBorrowedBooksRequest request)
    {
        var mostBorrowedBooksList = await _libraryDbContext.BorrowedBooks
            .Include(b => b.Book)
            .GroupBy(b => new { b.BookId, b.Book.Title, b.Book.ISBN, b.Book.Author })
            .Take(request.c)
            .Select(group => new MostBorrowedBook(group.Key.Title, group.Key.ISBN.Code, group.Key.Author, group.Count()))
            .OrderByDescending(b => b.TimeBorrowed)
            .ToListAsync();

        return new MostBorrowedBooksResponse(mostBorrowedBooksList);
    }
}
