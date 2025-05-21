using System.ComponentModel;
using System.Text.Json;
using Grpc.Core;
using Library.Reporting.Protos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Library.API.Tests;

[Category("Functional Tests")]
public class ReportEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly Mock<ReportingService.ReportingServiceClient> _mockReportingClient;

    public ReportEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _mockReportingClient = new Mock<ReportingService.ReportingServiceClient>();

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(_mockReportingClient.Object);
        
        var app = builder.Build();
        app.RegisterReportsEndpoints();

        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMostBorrowedBooks_ReturnsOk_WhenServiceCallSucceeds()
    {
        // Arrange
        var expectedResponse = new GetReportResponse
        {
            MostBorrowedBooksReponse = new MostBorrowedBooksResponse
            {
                MostBorrowedBooks = 
                {
                    new MostBorrowedBook 
                    { 
                        Title = "Test Book",
                        Author = "Test Author",
                        Isbn = "1234567890",
                        CopiesBorrowed = 5
                    }
                }
            }
        };

        _mockReportingClient
            .Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetReportResponse>(
                Task.FromResult(expectedResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }));

        // Act
        var response = await _client.GetAsync("/reports/most-borrowed-books?top=5");

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<MostBorrowedBook>>(content);
        
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Book", result[0].Title);
    }

    [Fact]
    public async Task GetBookStatus_ReturnsOk_WhenServiceCallSucceeds()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var expectedResponse = new GetReportResponse
        {
            BookStatusResponse = new BookStatusResponse
            {
                Title = "Test Book",
                TotalCopies = 10,
                CopiesBorrowed = 3,
                CopiesRemaining = 7
            }
        };

        _mockReportingClient
            .Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetReportResponse>(
                Task.FromResult(expectedResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }));

        // Act
        var response = await _client.GetAsync($"/reports/book-status/{bookId}");

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<BookStatusResponse>(content);
        
        Assert.NotNull(result);
        Assert.Equal("Test Book", result.Title);
        Assert.Equal(10, result.TotalCopies);
    }

    [Fact]
    public async Task GetMostActiveBookBorrowers_ReturnsOk_WhenServiceCallSucceeds()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var expectedResponse = new GetReportResponse
        {
            MostActiveBorrowerResponse = new MostActiveBorrowersResponse
            {
                Borrowers = 
                {
                    new MostActiveBorrower
                    {
                        BorrowerName = "Test User",
                        BooksBorrowed = 5
                    }
                }
            }
        };

        _mockReportingClient
            .Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetReportResponse>(
                Task.FromResult(expectedResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }));

        // Act
        var response = await _client.GetAsync(
            $"/reports/most-active-book-borrowers?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&count=5");

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MostActiveBorrowersResponse>(content);
        
        Assert.NotNull(result);
        Assert.Single(result.Borrowers);
        Assert.Equal("Test User", result.Borrowers[0].BorrowerName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    public async Task GetMostBorrowedBooks_ReturnsBadRequest_WhenInvalidTopValue(int top)
    {
        // Act
        var response = await _client.GetAsync($"/reports/most-borrowed-books?top={top}");

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
    }

    [Fact]
    public async Task GetMostBorrowedBooks_ReturnsError_WhenServiceFails()
    {
        // Arrange
        _mockReportingClient
            .Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetReportResponse>(
                Task.FromException<GetReportResponse>(new RpcException(new Status(StatusCode.Internal, "Service error"))),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

        // Act
        var response = await _client.GetAsync("/reports/most-borrowed-books?top=5");

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, (int)response.StatusCode);
    }

    [Fact]
    public async Task GetBookStatus_ReturnsNotFound_WhenBookDoesNotExist()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var expectedResponse = new GetReportResponse
        {
            Error = new () { Message = "Book not found" }
        };

        _mockReportingClient
            .Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetReportResponse>(
                Task.FromResult(expectedResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

        // Act
        var response = await _client.GetAsync($"/reports/book-status/{bookId}");

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
    }

    [Fact]
    public async Task GetUserBorrowedBooks_ReturnsOk_WhenServiceCallSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedResponse = new GetReportResponse
        {
            UserBorrowedBooksResponse = new UserBorrowedBooksResponse
            {
                Books = 
                {
                    new BorrowedBookInfo
                    {
                        Title = "Test Book",
                        Author = "Test Author",
                        BorrowedDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
                        ReturnedDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddDays(7))
                    }
                }
            }
        };

        _mockReportingClient
            .Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetReportResponse>(
                Task.FromResult(expectedResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

        // Act
        var response = await _client.GetAsync($"/reports/user-borrowed-books/{userId}?startDate=2024-01-01");

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<UserBorrowedBooksResponse>(content);
        
        Assert.NotNull(result);
        Assert.Single(result.Books);
    }

    [Fact]
    public async Task GetOtherBooksBorrowedBySameUsers_ReturnsOk_WhenServiceCallSucceeds()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var expectedResponse = new GetReportResponse
        {
            OtherBooksBorrowedBySameUsersReponse = new OtherBooksBorrowedBySameUsersResponse
            {
                Books = 
                {
                    new BorrowedBookInfo
                    {
                        Title = "Test Book",
                        Author = "Test Author",
                        BorrowedDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
                        ReturnedDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddDays(7))
                    }
                }
            }
        };

        _mockReportingClient
            .Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetReportResponse>(
                Task.FromResult(expectedResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

        // Act
        var response = await _client.GetAsync($"/reports/other-books-borrowed-by-same-users/{bookId}");

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<OtherBooksBorrowedBySameUsersResponse>(content);
        
        Assert.NotNull(result);
        Assert.Single(result.Books);
    }

    [Fact]
    public async Task GetBookReadRate_ReturnsOk_WhenServiceCallSucceeds()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var expectedResponse = new GetReportResponse
        {
            BookReadRateResponse = new BookReadRateResponse
            {
                AverageReadRate = 0.75f
            }
        };

        _mockReportingClient
            .Setup(x => x.GetReportAsync(
                It.IsAny<GetReportRequest>(),
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetReportResponse>(
                Task.FromResult(expectedResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

        // Act
        var response = await _client.GetAsync($"/reports/book-read-rate/{bookId}");

        // Assert
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<BookReadRateResponse>(content);
        
        Assert.NotNull(result);
        Assert.Equal(0.75f, result.AverageReadRate);
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("")]
    public async Task GetBookStatus_ReturnsBadRequest_WhenInvalidBookId(string bookId)
    {
        // Act
        var response = await _client.GetAsync($"/reports/book-status/{bookId}");

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
    }

    [Fact]
    public async Task GetMostActiveBookBorrowers_ReturnsBadRequest_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);

        // Act
        var response = await _client.GetAsync(
            $"/reports/most-active-book-borrowers?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&count=5");

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}
