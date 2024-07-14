using Library.Reporting.DataAccess;
using Library.Reporting.Protos;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Error = Library.Reporting.Protos.Error;
using Google.Protobuf.WellKnownTypes;

namespace Library.Reporting.Service.ReportHandlers;

public class UserBorrowedBooksReportHandler
{
    private readonly LibraryDbContext _libraryDbContext;

    public UserBorrowedBooksReportHandler(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<OneOf<UserBorrowedBooksResponse, Error>> ExecuteAsync(UserBorrowedBooksRequest request, CancellationToken cancellationToken)
    {
        var borrowedBooks = await _libraryDbContext.BorrowedBooks
            .Where(bb => bb.BorrowerId == Guid.Parse(request.BorrowerId) && bb.BorrowedDate >= request.StartDate.ToDateTime() && bb.BorrowedDate <= request.EndDate.ToDateTime())
            .Include(bb => bb.Book)
            .Select(bb => new BorrowedBookInfo
            {
                Title = bb.Book.Title,
                Author = bb.Book.Author,
                BorrowedDate = Timestamp.FromDateTime(bb.BorrowedDate.ToUniversalTime()),
                ReturnedDate = bb.ReturnedDate.HasValue ? Timestamp.FromDateTime(bb.ReturnedDate.Value.ToUniversalTime()) : null
            })
            .ToListAsync(cancellationToken);

        return new UserBorrowedBooksResponse { Books = { borrowedBooks } };
    }
}
