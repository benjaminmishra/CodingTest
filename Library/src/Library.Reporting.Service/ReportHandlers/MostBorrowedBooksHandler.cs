using Library.Reporting.DataAccess;
using Microsoft.EntityFrameworkCore;
using Library.Reporting.Protos;
using OneOf;

namespace Library.Reporting.Service.ReportHandlers;

public class MostBorrowedBookReportHandler
{
    private readonly LibraryDbContext _libraryDbContext;

    public MostBorrowedBookReportHandler(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<OneOf<MostBorrowedBooksResponse, Error>> ExecuteAsync(MostBorrowedBooksRequest request, CancellationToken cancellationToken)
    {
        if(request.Count <= 0)
            return new Error { Message = "Count cannot be less than or equal to 0"};

        var mostBorrowedBooksList = await _libraryDbContext.BorrowedBooks
            .Include(b => b.Book)
            .GroupBy(b => new { b.BookId, b.Book.Title, b.Book.ISBN, b.Book.Author })
            .Take(request.Count)
            .Select(group => new MostBorrowedBook
            {
                Title = group.Key.Title,
                Isbn = group.Key.ISBN.Code,
                Author = group.Key.Author,
                CopiesBorrowed = group.Count()
            })
            .OrderByDescending(b => b.CopiesBorrowed)
            .ToListAsync(cancellationToken);

        return new MostBorrowedBooksResponse { MostBorrowedBooks = { mostBorrowedBooksList } };
    }
}
