using System.Net;
using System.Net.Http.Json;
using Google.Protobuf.WellKnownTypes;
using Library.Reporting.Protos;
using Library.Tests.Core;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.API.Tests;

[UnitTests]
public class ReportEndpointsTests : IClassFixture<ReportingApiWebApplcationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly Mock<ReportingService.ReportingServiceClient> _mockClient;

    public ReportEndpointsTests(ReportingApiWebApplcationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _mockClient = factory.MockReportingClient;
    }

    [Theory]
    [InlineData("/reports/most-borrowed-books?top=0", HttpStatusCode.BadRequest, null)]
    [InlineData("/reports/book-status/not-a-guid", HttpStatusCode.NotFound, null)]
    [InlineData("/reports/most-active-book-borrowers?startDate=2025-02-01&endDate=2025-01-01&count=5", HttpStatusCode.BadRequest, "StartDate cannot be greater than end date")]
    [InlineData("/reports/most-active-book-borrowers?startDate=2025-01-01&endDate=2025-02-01&count=-1", HttpStatusCode.BadRequest, "Count cannot be negetive")]
    public async Task InvalidParameters_ReturnsExpected(string url, HttpStatusCode status, string? expectedMessage)
    {
        var response = await _client.GetAsync(url);

        Assert.NotNull(response);
        Assert.Equal(status, response.StatusCode);
        
        if (expectedMessage is not null)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains(expectedMessage, content);
        }
    }

    [Fact]
    public async Task GetMostBorrowedBooks_ReturnsArray_WhenSuccessful()
    {
        var books = new List<MostBorrowedBook>
        {
            new() { Title = "A", Author = "Auth", Isbn = "1", CopiesBorrowed = 10 }
        };
        var grpcResponse = new GetReportResponse { MostBorrowedBooksReponse = new MostBorrowedBooksResponse() };
        grpcResponse.MostBorrowedBooksReponse.MostBorrowedBooks.AddRange(books);

        _mockClient.SetupGrpc(grpcResponse);
        var result = await _client.GetFromJsonAsync<MostBorrowedBook[]>("/reports/most-borrowed-books?top=5");

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("A", result[0].Title);
    }

    [Fact]
    public async Task GetBookStatus_ReturnsObject_WhenSuccessful()
    {
        var expected = new BookStatusResponse { Title = "B", TotalCopies = 5, CopiesBorrowed = 2, CopiesRemaining = 3 };
        var grpcResponse = new GetReportResponse { BookStatusResponse = expected };

        _mockClient.SetupGrpc(grpcResponse);
        var result = await _client.GetFromJsonAsync<BookStatusResponse>("/reports/book-status/00000000-0000-0000-0000-000000000000");

        Assert.Equal(5, result?.TotalCopies);
    }

    [Fact]
    public async Task GetMostActiveBorrowers_ReturnsObject_WhenSuccessful()
    {
        var resp = new MostActiveBorrowersResponse();
        resp.Borrowers.Add(
            new MostActiveBorrower 
            { 
                BorrowerName = "U", 
                BooksBorrowed = 7 
            });

        var grpcResponse = new GetReportResponse { MostActiveBorrowerResponse = resp };

        _mockClient.SetupGrpc(grpcResponse);
        var result = await _client.GetFromJsonAsync<MostActiveBorrower[]>(
            "/reports/most-active-book-borrowers?startDate=2025-01-01&endDate=2025-01-31&count=5");

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetUserBorrowedBooks_ReturnsObject_WhenSuccessful()
    {
        var resp = new UserBorrowedBooksResponse();
        resp.Books.Add(
            new BorrowedBookInfo 
            { 
                Title = "X", 
                Author = "Y", 
                BorrowedDate = Timestamp.FromDateTime(DateTime.UtcNow), 
                ReturnedDate = Timestamp.FromDateTime(DateTime.UtcNow) 
            });
        var grpcResponse = new GetReportResponse { UserBorrowedBooksResponse = resp };

        _mockClient.SetupGrpc(grpcResponse);
        var result = await _client.GetFromJsonAsync<BorrowedBookInfo[]>(
            "/reports/user-borrowed-books/00000000-0000-0000-0000-000000000000?startDate=2025-01-01");

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetOtherBooksBorrowedBySameUsers_ReturnsObject_WhenSuccessful()
    {
        var resp = new OtherBooksBorrowedBySameUsersResponse();
        resp.Books.Add(
            new BorrowedBookInfo 
            { 
                Title = "P", 
                Author = "Q", 
                BorrowedDate = Timestamp.FromDateTime(DateTime.UtcNow), 
                ReturnedDate = Timestamp.FromDateTime(DateTime.UtcNow) 
            });

        var grpcResponse = new GetReportResponse { OtherBooksBorrowedBySameUsersReponse = resp };

        _mockClient.SetupGrpc(grpcResponse);
        var result = await _client.GetFromJsonAsync<BorrowedBookInfo[]>(
            "/reports/other-books-borrowed-by-same-users/00000000-0000-0000-0000-000000000000");

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetBookReadRate_ReturnsObject_WhenSuccessful()
    {
        var expected = new BookReadRateResponse { AverageReadRate = 0.42f };
        var grpcResponse = new GetReportResponse { BookReadRateResponse = expected };

        _mockClient.SetupGrpc(grpcResponse);
        var result = await _client.GetFromJsonAsync<BookReadRateResponse>(
            "/reports/book-read-rate/00000000-0000-0000-0000-000000000000");

        Assert.NotNull(result);
        Assert.Equal(0.42f, result.AverageReadRate);
    }

    [Fact]
    public async Task ServiceError_ReturnsProblemDetails()
    {
        _mockClient.SetupError();
        var response = await _client.GetAsync("/reports/most-borrowed-books?top=1");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Contains("Error", json?.Detail);
    }

    public void Dispose() => _client.Dispose();
}
