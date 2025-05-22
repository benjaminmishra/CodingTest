using Library.Reporting.DataAccess;
using Library.Reporting.Models;
using Library.Reporting.Protos;
using Library.Reporting.Service.ReportHandlers;
using Library.Tests.Core;

namespace Library.Reporting.Service.Tests;

[IntegrationTests]
public class BookReadRateReportHandlerTests : IDisposable
{
    private readonly DbFixture _fixture;
    private readonly LibraryDbContext _context;

    public BookReadRateReportHandlerTests()
    {
        _fixture = new DbFixture();
        _context = _fixture.DbContext;
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnAverageReadRate()
    {
        // Arrange
        var handler = new BookReadRateReportHandler(_context);
        var request = new BookReadRateRequest { BookId = _context.Books.First().Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BookReadRateResponse>(result.Value);
        var response = result.AsT0;
        Assert.True(response.AverageReadRate > 0);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnError_WhenNoRecordsFound()
    {
        // Arrange
        var handler = new BookReadRateReportHandler(_context);
        var request = new BookReadRateRequest { BookId = Guid.NewGuid().ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<Error>(result.Value);
        var error = result.AsT1;
        Assert.Equal("No completed borrow records found for the specified book", error.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnError_WhenBookIdIsInvalid()
    {
        // Arrange
        var handler = new BookReadRateReportHandler(_context);
        var request = new BookReadRateRequest { BookId = Guid.NewGuid().ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<Error>(result.Value);
        var error = result.AsT1;
        Assert.Equal("No completed borrow records found for the specified book", error.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleNullReturnedDate()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book 2",
            Pages = 150,
            Author = "Author2",
            ISBN = new Isbn("some123IsbnNumber"),
            CopiesCount = 2
        };
        await _context.Books.AddAsync(book);
        await _context.BorrowedBooks.AddAsync(
            new BorrowedBook
            {
                BookId = book.Id,
                Book = book,
                BorrowedDate = DateTime.Now.AddDays(-10),
                ReturnedDate = null,
                Borrower = new Borrower { Name = "User1" }
            }
        );
        await _context.SaveChangesAsync();

        var handler = new BookReadRateReportHandler(_context);
        var request = new BookReadRateRequest { BookId = book.Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<Error>(result.Value);
        var error = result.AsT1;
        Assert.Equal("No completed borrow records found for the specified book", error.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCalculateAverageReadRateCorrectly()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book 3",
            Pages = 200,
            Author = "Author2",
            ISBN = new Isbn("TestIsbn1234"),
            CopiesCount = 2
        };
        await _context.Books.AddAsync(book);
        await _context.BorrowedBooks.AddRangeAsync(
            new BorrowedBook
            {
                BookId = book.Id,
                Book = book,
                BorrowedDate = DateTime.Now.AddDays(-15),
                ReturnedDate = DateTime.Now.AddDays(-10),
                Borrower = new Borrower { Name = "Test User" }
            }, // 5 days
            new BorrowedBook
            {
                BookId = book.Id,
                Book = book,
                BorrowedDate = DateTime.Now.AddDays(-25),
                ReturnedDate = DateTime.Now.AddDays(-20),
                Borrower = new Borrower { Name = "Test User 1" }
            } // 10 days
        );
        await _context.SaveChangesAsync();

        var handler = new BookReadRateReportHandler(_context);
        var request = new BookReadRateRequest { BookId = book.Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BookReadRateResponse>(result.Value);
        var response = result.AsT0;
        var expectedAverageReadRate = Math.Round(200.0 / 5, 2); // (200 pages / (5 days + 5 days) / 2)
        Assert.Equal(expectedAverageReadRate, response.AverageReadRate, 2);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}