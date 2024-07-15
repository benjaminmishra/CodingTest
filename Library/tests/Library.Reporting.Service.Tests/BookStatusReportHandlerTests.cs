using Library.Reporting.DataAccess;
using Library.Reporting.Models;
using Library.Reporting.Protos;
using Library.Reporting.Service.ReportHandlers;
using System.ComponentModel;

namespace Library.Reporting.Service.Tests;

[Category("Integration Tests")]
public class BookStatusReportHandlerTests : IDisposable
{
    private readonly DbFixture _fixture;
    private readonly LibraryDbContext _context;

    public BookStatusReportHandlerTests()
    {
        _fixture = new DbFixture();
        _context = _fixture.DbContext;
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnBookStatus()
    {
        // Arrange
        var handler = new BookStatusReportHandler(_context);
        var request = new BooksStatusRequest { BookId = _context.Books.First().Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BookStatusResponse>(result.Value);
        var response = result.AsT0;
        Assert.Equal("Test Book 1", response.Title);
        Assert.Equal(5, response.TotalCopies);
        Assert.Equal(1, response.CopiesBorrowed);
        Assert.Equal(4, response.CopiesRemaining);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnError_WhenBookIdIsInvalidGuid()
    {
        // Arrange
        var handler = new BookStatusReportHandler(_context);
        var request = new BooksStatusRequest { BookId = "invalid-guid" };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<Error>(result.Value);
        var error = result.AsT1;
        Assert.Equal("Book Id is an invalid Guid", error.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnError_WhenNoBookFound()
    {
        // Arrange
        var handler = new BookStatusReportHandler(_context);
        var request = new BooksStatusRequest { BookId = Guid.NewGuid().ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<Error>(result.Value);
        var error = result.AsT1;
        Assert.Equal("No book found with the given id", error.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleNoBorrowedBooks()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Test Book 2",
            CopiesCount = 5,
            Author = "Author2",
            Pages = 1200,
            ISBN = new Isbn("123456")
        };
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        var handler = new BookStatusReportHandler(_context);
        var request = new BooksStatusRequest { BookId = book.Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BookStatusResponse>(result.Value);
        var response = result.AsT0;
        Assert.Equal("Test Book 2", response.Title);
        Assert.Equal(5, response.TotalCopies);
        Assert.Equal(0, response.CopiesBorrowed);
        Assert.Equal(5, response.CopiesRemaining);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleAllCopiesBorrowed()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Test Book 3",
            CopiesCount = 2,
            Author = "Author3",
            Pages = 400,
            ISBN = new Isbn("3454675")
        };
        await _context.Books.AddAsync(book);
        await _context.BorrowedBooks.AddRangeAsync(
            new BorrowedBook
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                BorrowedDate = DateTime.Now.AddDays(-10),
                ReturnedDate = null,
                Book = book,
                Borrower = new Borrower { Name = "User1" }
            },
            new BorrowedBook
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                BorrowedDate = DateTime.Now.AddDays(-5),
                ReturnedDate = null,
                Book = book,
                Borrower = new Borrower { Name = "User2" }
            }
        );
        await _context.SaveChangesAsync();

        var handler = new BookStatusReportHandler(_context);
        var request = new BooksStatusRequest { BookId = book.Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BookStatusResponse>(result.Value);
        var response = result.AsT0;
        Assert.Equal("Test Book 3", response.Title);
        Assert.Equal(2, response.TotalCopies);
        Assert.Equal(2, response.CopiesBorrowed);
        Assert.Equal(0, response.CopiesRemaining);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleMultipleBorrowedAndReturnedCopies()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Test Book 4",
            CopiesCount = 5,
            Author = "Author4",
            Pages = 1200,
            ISBN = new Isbn("987654")
        };
        await _context.Books.AddAsync(book);
        await _context.BorrowedBooks.AddRangeAsync(
            new BorrowedBook
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                BorrowedDate = DateTime.Now.AddDays(-10),
                ReturnedDate = DateTime.Now.AddDays(-5),
                Book = book,
                Borrower = new Borrower { Name = "User 4" }
            },
            new BorrowedBook
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                BorrowedDate = DateTime.Now.AddDays(-20),
                ReturnedDate = null,
                Book = book,
                Borrower = new Borrower { Name = "User 5" }
            }
        );
        await _context.SaveChangesAsync();

        var handler = new BookStatusReportHandler(_context);
        var request = new BooksStatusRequest { BookId = book.Id.ToString() };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<BookStatusResponse>(result.Value);
        var response = result.AsT0;
        Assert.Equal("Test Book 4", response.Title);
        Assert.Equal(5, response.TotalCopies);
        Assert.Equal(1, response.CopiesBorrowed);
        Assert.Equal(4, response.CopiesRemaining);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}