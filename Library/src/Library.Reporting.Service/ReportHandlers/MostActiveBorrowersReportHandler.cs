using Library.Reporting.DataAccess;
using Library.Reporting.Protos;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Error = Library.Reporting.Protos.Error;

namespace Library.Reporting.Service.ReportHandlers;

public class MostActiveBorrowersReportHandler
{
    private readonly LibraryDbContext _libraryDbContext;

    public MostActiveBorrowersReportHandler(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    public async Task<OneOf<MostActiveBorrowersResponse, Error>> ExecuteAsync(MostActiveBorrowersRequest request, CancellationToken cancellationToken)
    {
        var mostActiveBorrowers = await _libraryDbContext.BorrowedBooks
            .Where(bb => bb.BorrowedDate >= request.StartDate.ToDateTime() && bb.BorrowedDate <= request.EndDate.ToDateTime())
            .GroupBy(bb => bb.Borrower)
            .Select(group => new MostActiveBorrower
            {
                BorrowerName = group.Key.Name,
                BooksBorrowed = group.Count()
            })
            .OrderByDescending(b => b.BooksBorrowed)
            .Take(request.Count)
            .ToListAsync(cancellationToken);

        return new MostActiveBorrowersResponse { Borrowers = { mostActiveBorrowers } };
    }
}