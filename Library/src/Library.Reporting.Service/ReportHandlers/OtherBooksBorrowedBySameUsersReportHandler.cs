using Library.Reporting.DataAccess;
using Library.Reporting.Protos;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Error = Library.Reporting.Protos.Error;
using Google.Protobuf.WellKnownTypes;

namespace Library.Reporting.Service.ReportHandlers;

public class OtherBooksBorrowedBySameUsersReportHandler
{
    private readonly LibraryDbContext _libraryDbContext;

    public OtherBooksBorrowedBySameUsersReportHandler(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<OneOf<OtherBooksBorrowedBySameUsersResponse, Error>> ExecuteAsync(OtherBooksBorrowedBySameUsersRequest request, CancellationToken cancellationToken)
    {
        var bookBorrowers = _libraryDbContext.BorrowedBooks
            .Where(bb => bb.BookId == Guid.Parse(request.BookId))
            .Select(bb => bb.BorrowerId)
            .Distinct();

        var otherBooks = await _libraryDbContext.BorrowedBooks
            .Where(bb => bookBorrowers.Contains(bb.BorrowerId) && bb.BookId != Guid.Parse(request.BookId))
            .Include(bb => bb.Book)
            .Select(bb => new BorrowedBookInfo
            {
                Title = bb.Book.Title,
                Author = bb.Book.Author,
                BorrowedDate = Timestamp.FromDateTime(bb.BorrowedDate.ToUniversalTime()),
                ReturnedDate = bb.ReturnedDate.HasValue ? Timestamp.FromDateTime(bb.ReturnedDate.Value.ToUniversalTime()) : null
            })
            .Distinct()
            .ToListAsync(cancellationToken);

        return new OtherBooksBorrowedBySameUsersResponse { Books = { otherBooks } };
    }
}
