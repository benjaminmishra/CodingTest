using Library.Reporting.Types;
using Library.Reporting.DataAccess;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Linq;

namespace Library.Reporting.Service.ReportHandlers;

public class MostBorrowedBookReportHandler : IRequestHandler<MostBorrowedBooksRequest,MostBorrowedBooksResponse>
{
    private readonly LibraryDbContext _libraryDbContext;

    public MostBorrowedBookReportHandler(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<MostBorrowedBooksResponse> Handle(MostBorrowedBooksRequest request, CancellationToken cancellationToken)
    {
        var mostBorrowedBooksList = await _libraryDbContext.BorrowedBooks
            .Include(b => b.Book)
            .GroupBy(b => new { b.BookId, b.Book.Title, b.Book.ISBN, b.Book.Author })
            .Take(request.Count)
            .Select(group => new MostBorrowedBook(group.Key.Title, group.Key.ISBN.Code, group.Key.Author, group.Count()))
            .OrderByDescending(b => b.TimeBorrowed)
            .ToListAsync(cancellationToken);

        return new MostBorrowedBooksResponse(mostBorrowedBooksList);
    }
}
