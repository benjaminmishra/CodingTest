using Library.Reporting.DataAccess;
using Library.Reporting.Models;
using Library.Reporting.Protos;
using Library.Reporting.Service.ReportHandlers;
using Library.Tests.Core;

namespace Library.Reporting.Service.Tests;

[IntegrationTests]
public class OtherBooksBorrowedBySameUsersReportHandlerTests : IDisposable
{
    private readonly DbFixture _fixture;
    private readonly LibraryDbContext _context;

    public OtherBooksBorrowedBySameUsersReportHandlerTests()
    {
        _fixture = new DbFixture();
        _context = _fixture.DbContext;
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnOtherBooksBorrowedBySameUsers()
    {
        // Arrange
        // Seed data
        var book3 = new Book
        {
            Title = "Test Book 3",
            Author = "Author 3",
            Pages = 500,
            ISBN = new Isbn("123456-345345"),
            CopiesCount = 6
        };
        _context.Books.Add(book3);
        var borrower1 = _context.Borrowers.First(x => x.Name == "John Doe");

        var borrowedBook = new BorrowedBook
        {
            BookId = book3.Id,
            ReturnedDate = DateTime.UtcNow.AddMonths(1),
            BorrowedDate = DateTime.UtcNow.AddDays(-1),
            BorrowerId = borrower1.Id,
            Book = book3,
            Borrower = borrower1
        };
        _context.BorrowedBooks.Add(borrowedBook);
        await _context.SaveChangesAsync();

        var handler = new OtherBooksBorrowedBySameUsersReportHandler(_context);
        var request = new OtherBooksBorrowedBySameUsersRequest { BookId = _context.Books.First(x => x.Title == "Test Book 1").Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<OtherBooksBorrowedBySameUsersResponse>(result.Value);
        var response = result.AsT0;
        Assert.Single(response.Books);
        Assert.Equal("Test Book 3", response.Books[0].Title);
        Assert.Equal("Author 3", response.Books[0].Author);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnError_WhenInvalidBookId()
    {
        // Arrange
        var handler = new OtherBooksBorrowedBySameUsersReportHandler(_context);
        var request = new OtherBooksBorrowedBySameUsersRequest { BookId = Guid.NewGuid().ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<OtherBooksBorrowedBySameUsersResponse>(result.Value);
        var response = result.AsT0;
        Assert.Empty(response.Books);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoOtherBooksBorrowed()
    {
        // Arrange
        var handler = new OtherBooksBorrowedBySameUsersReportHandler(_context);
        var request = new OtherBooksBorrowedBySameUsersRequest { BookId = _context.Books.First(x => x.Title == "Test Book 2").Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<OtherBooksBorrowedBySameUsersResponse>(result.Value);
        var response = result.AsT0;
        Assert.Empty(response.Books);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleMultipleBorrowers()
    {
        var book = _context.Books.First(b => b.Title == "Test Book 1");
        var borrower1 = new Borrower { Name = "John Smith" };
        _context.Borrowers.Add(borrower1);
        var borrowedBook = new BorrowedBook
        {
            BookId = book.Id,
            ReturnedDate = DateTime.UtcNow.AddMonths(1),
            BorrowedDate = DateTime.UtcNow.AddDays(-1),
            BorrowerId = borrower1.Id,
            Book = book,
            Borrower = borrower1
        };
        _context.BorrowedBooks.Add(borrowedBook);
        await _context.SaveChangesAsync();

        // Arrange
        var handler = new OtherBooksBorrowedBySameUsersReportHandler(_context);


        var request = new OtherBooksBorrowedBySameUsersRequest { BookId = book.Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<OtherBooksBorrowedBySameUsersResponse>(result.Value);
        var response = result.AsT0;

        Assert.Empty(response.Books);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}