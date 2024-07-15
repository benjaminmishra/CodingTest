using Google.Protobuf.WellKnownTypes;
using Library.Reporting.DataAccess;
using Library.Reporting.Models;
using Library.Reporting.Protos;
using Library.Reporting.Service.ReportHandlers;
using System.ComponentModel;

namespace Library.Reporting.Service.Tests;

[Category("Integration Tests")]
public class UserBorrowedBooksReportHandlerTests : IDisposable
{
    private readonly DbFixture _fixture;
    private readonly LibraryDbContext _context;

    public UserBorrowedBooksReportHandlerTests()
    {
        _fixture = new DbFixture();
        _context = _fixture.DbContext;
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnBorrowedBooksByUserWithinDateRange()
    {
        // Arrange
        var handler = new UserBorrowedBooksReportHandler(_context);
        var request = new UserBorrowedBooksRequest
        {
            BorrowerId = _context.Borrowers.First(b => b.Name == "John Doe").Id.ToString(),
            StartDate = Timestamp.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
            EndDate = Timestamp.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<UserBorrowedBooksResponse>(result.Value);
        var response = result.AsT0;
        Assert.Equal(1, response.Books.Count);
        Assert.Contains(response.Books, b => b.Title == "Test Book 1" && b.Author == "Author 1");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoBooksBorrowedWithinDateRange()
    {
        // Arrange
        var handler = new UserBorrowedBooksReportHandler(_context);
        var request = new UserBorrowedBooksRequest
        {
            BorrowerId = _context.Borrowers.First().Id.ToString(),
            StartDate = Timestamp.FromDateTime(DateTime.UtcNow.AddMonths(-2)),
            EndDate = Timestamp.FromDateTime(DateTime.UtcNow.AddMonths(-1))
        };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<UserBorrowedBooksResponse>(result.Value);
        var response = result.AsT0;
        Assert.Empty(response.Books);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnError_WhenInvalidBorrowerId()
    {
        // Arrange
        var handler = new UserBorrowedBooksReportHandler(_context);
        var request = new UserBorrowedBooksRequest
        {
            BorrowerId = Guid.NewGuid().ToString(),
            StartDate = Timestamp.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
            EndDate = Timestamp.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<UserBorrowedBooksResponse>(result.Value);
        var response = result.AsT0;
        Assert.Empty(response.Books);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleEdgeCase_NoBorrowedBooks()
    {
        // Arrange
        var handler = new UserBorrowedBooksReportHandler(_context);
        var newBorrower = new Borrower { Id = Guid.NewGuid(), Name = "Jane Doe" };
        await _context.Borrowers.AddAsync(newBorrower);
        await _context.SaveChangesAsync();

        var request = new UserBorrowedBooksRequest
        {
            BorrowerId = newBorrower.Id.ToString(),
            StartDate = Timestamp.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
            EndDate = Timestamp.FromDateTime(DateTime.UtcNow)
        };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<UserBorrowedBooksResponse>(result.Value);
        var response = result.AsT0;
        Assert.Empty(response.Books);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}