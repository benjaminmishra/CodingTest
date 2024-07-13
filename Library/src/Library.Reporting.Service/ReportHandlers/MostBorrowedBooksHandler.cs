using Library.Reporting.Types;
using Library.Reporting.DataAccess;
using Microsoft.EntityFrameworkCore;
using MediatR;
using System.Linq;
using Library.Reporting.Types.Protos;

namespace Library.Reporting.Service.ReportHandlers;

public class MostBorrowedBookReportHandler
{
    private readonly LibraryDbContext _libraryDbContext;

    public MostBorrowedBookReportHandler(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<MostBorrowedBooksResponse> ExecuteAsync(MostBorrowedBooksRequest request, CancellationToken cancellationToken)
    {
        var mostBorrowedBooksList = await _libraryDbContext.BorrowedBooks
            .Include(b => b.Book)
            .GroupBy(b => new { b.BookId, b.Book.Title, b.Book.ISBN, b.Book.Author })
            .Take(request.Count)
            .Select(group => new MostBorrowedBook
            {
                Title = group.Key.Title,
                ISBN = group.Key.ISBN.Code,
                Author = group.Key.Author,
                CopiesBorrowed = group.Count()
            })
            .OrderByDescending(b => b.CopiesBorrowed)
            .ToListAsync(cancellationToken);

        return new MostBorrowedBooksResponse { MostBorrowedBooks = { mostBorrowedBooksList } };
    }
}
