using Library.Reporting.DataAccess;
using Library.Reporting.Protos;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace Library.Reporting.Service.ReportHandlers
{
    public class BookStatusReportHandler
    {
        private readonly LibraryDbContext _libraryDbContext;

        public BookStatusReportHandler(LibraryDbContext libraryDbContext)
        {
            _libraryDbContext = libraryDbContext;
        }

        public async Task<OneOf<BookStatusResponse, Error>> ExecuteAsync(BooksStatusRequest request, CancellationToken cancellationToken)
        {
            if(!Guid.TryParse(request.BookId, out Guid bookGuid))
                return new Error { Message = "Book Id is an invalid Guid"};

            var bookStatus = await _libraryDbContext.Books
                .Where(b => b.Id == bookGuid)
                .Select(b => new BookStatusResponse
                {
                    Title = b.Title,
                    TotalCopies = b.CopiesCount,
                    CopiesBorrowed = _libraryDbContext.BorrowedBooks.Count(bb => bb.BookId == bookGuid && bb.ReturnedDate == null),
                    CopiesRemaining = b.CopiesCount - _libraryDbContext.BorrowedBooks.Count(bb => bb.BookId == bookGuid && bb.ReturnedDate == null)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if(bookStatus is null)
                return new Error { Message = "No book found with the given id"};
            
            return bookStatus;
        }
    }
}
