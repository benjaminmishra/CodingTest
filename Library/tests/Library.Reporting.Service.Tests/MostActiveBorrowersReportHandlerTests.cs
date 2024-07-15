using Library.Reporting.DataAccess;
using Library.Reporting.Protos;
using Library.Reporting.Service.ReportHandlers;
using System.ComponentModel;

namespace Library.Reporting.Service.Tests;

[Category("Integration Tests")]
public class MostActiveBorrowersReportHandlerTests
{
    private readonly DbFixture _fixture;
    private readonly LibraryDbContext _context;

    public MostActiveBorrowersReportHandlerTests()
    {
        _fixture = new DbFixture();
        _context = _fixture.DbContext;
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnMostActiveBorrowers()
    {
        // Arrange
        var handler = new MostActiveBorrowersReportHandler(_context);
        var request = new MostActiveBorrowersRequest
        {
            StartDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-30).ToUniversalTime()),
            EndDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.ToUniversalTime()),
            Count = 2
        };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<MostActiveBorrowersResponse>(result.Value);
        var response = result.AsT0;
        Assert.Equal(2, response.Borrowers.Count);
        Assert.Equal("John Doe", response.Borrowers[0].BorrowerName);
        Assert.Equal(1, response.Borrowers[0].BooksBorrowed);
        Assert.Equal("Jane Doe", response.Borrowers[1].BorrowerName);
        Assert.Equal(1, response.Borrowers[1].BooksBorrowed);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoBorrowersFound()
    {
        // Arrange
        var handler = new MostActiveBorrowersReportHandler(_context);
        var request = new MostActiveBorrowersRequest
        {
            StartDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-5).ToUniversalTime()),
            EndDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-1).ToUniversalTime()),
            Count = 2
        };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<MostActiveBorrowersResponse>(result.Value);
        var response = result.AsT0;
        Assert.Empty(response.Borrowers);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnLimitedNumberOfBorrowers()
    {
        // Arrange
        var handler = new MostActiveBorrowersReportHandler(_context);
        var request = new MostActiveBorrowersRequest
        {
            StartDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddDays(-30).ToUniversalTime()),
            EndDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.ToUniversalTime()),
            Count = 1
        };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<MostActiveBorrowersResponse>(result.Value);
        var response = result.AsT0;
        Assert.Single(response.Borrowers);
        Assert.Equal("John Doe", response.Borrowers[0].BorrowerName);
        Assert.Equal(1, response.Borrowers[0].BooksBorrowed);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleNoBorrowersWithinDateRange()
    {
        // Arrange
        var handler = new MostActiveBorrowersReportHandler(_context);
        var request = new MostActiveBorrowersRequest
        {
            StartDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-1).ToUniversalTime()),
            EndDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddMonths(-11).ToUniversalTime()),
            Count = 2
        };

        // Act
        var result = await handler.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.IsType<MostActiveBorrowersResponse>(result.Value);
        var response = result.AsT0;
        Assert.Empty(response.Borrowers);
    }
}