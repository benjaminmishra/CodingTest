using Library.Reporting.DataAccess;
using Library.Reporting.Protos;
using Library.Reporting.Service.ReportHandlers;

namespace Library.Reporting.Service.Tests;

public class MostBorrowedBookReportHandlerTests : IDisposable
{
    private readonly DbFixture _fixture;
    private readonly LibraryDbContext _context;

    public MostBorrowedBookReportHandlerTests()
    {
        _fixture = new DbFixture();
        _context = _fixture.DbContext;
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnMostBorrowedBooks()
    {
        // Arrange
        var handler = new MostBorrowedBookReportHandler(_context);
        var request = new MostBorrowedBooksRequest { Count = 2 };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<MostBorrowedBooksResponse>(result.Value);
        var response = result.AsT0;
        Assert.Equal(2, response.MostBorrowedBooks.Count);
        Assert.Equal("Test Book 1", response.MostBorrowedBooks[0].Title);
        Assert.Equal("Author 1", response.MostBorrowedBooks[0].Author);
        Assert.Equal(1, response.MostBorrowedBooks[0].CopiesBorrowed);

        Assert.Equal("Test Book 2", response.MostBorrowedBooks[1].Title);
        Assert.Equal("Author 2", response.MostBorrowedBooks[1].Author);
        Assert.Equal(1, response.MostBorrowedBooks[1].CopiesBorrowed);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnError_WhenCountIsLessThanOrEqualToZero()
    {
        // Arrange
        var handler = new MostBorrowedBookReportHandler(_context);
        var request = new MostBorrowedBooksRequest { Count = 0 };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<Error>(result.Value);
        var error = result.AsT1;
        Assert.Equal("Count cannot be less than or equal to 0", error.Message);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnLimitedNumberOfBooks()
    {
        // Arrange
        var handler = new MostBorrowedBookReportHandler(_context);
        var request = new MostBorrowedBooksRequest { Count = 1 };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<MostBorrowedBooksResponse>(result.Value);
        var response = result.AsT0;
        Assert.Single(response.MostBorrowedBooks);
        Assert.Equal("Test Book 1", response.MostBorrowedBooks[0].Title);
        Assert.Equal("Author 1", response.MostBorrowedBooks[0].Author);
        Assert.Equal(1, response.MostBorrowedBooks[0].CopiesBorrowed);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleSingleBorrowedBook()
    {
        // Arrange
        var handler = new MostBorrowedBookReportHandler(_context);

        var request = new MostBorrowedBooksRequest { Count = 2 };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<MostBorrowedBooksResponse>(result.Value);
        var response = result.AsT0;
        Assert.Equal(2, response.MostBorrowedBooks.Count);
        Assert.Equal("Test Book 1", response.MostBorrowedBooks[0].Title);
        Assert.Equal("Author 1", response.MostBorrowedBooks[0].Author);
        Assert.Equal(1, response.MostBorrowedBooks[0].CopiesBorrowed);

        Assert.Equal("Test Book 2", response.MostBorrowedBooks[1].Title);
        Assert.Equal("Author 2", response.MostBorrowedBooks[1].Author);
        Assert.Equal(1, response.MostBorrowedBooks[1].CopiesBorrowed);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}