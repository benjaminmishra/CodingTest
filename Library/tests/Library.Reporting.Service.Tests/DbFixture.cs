using Library.Reporting.DataAccess;
using Library.Reporting.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Library.Reporting.Service.Tests;

public class DbFixture : IDisposable
{
    public LibraryDbContext DbContext { get; }

    private readonly MsSqlContainer _mssqlContainer;

    public DbFixture()
    {
        // TODO: Load password from secret store/ configuration
        _mssqlContainer = new MsSqlBuilder()
            .WithPassword("yourStrong(!)Password")
            .Build();

        _mssqlContainer.StartAsync().Wait();

        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlServer(_mssqlContainer.GetConnectionString())
            .Options;

        DbContext = new LibraryDbContext(options);

        SeedData();
    }

    private void SeedData()
    {
        DbContext.Database.EnsureCreated();

        // Seed data
        var book1 = new Book
        {
            Title = "Test Book 1",
            Author = "Author 1",
            Pages = 500,
            ISBN = new Isbn("123456-345345"),
            CopiesCount = 5
        };
        var book2 = new Book
        {
            Title = "Test Book 2",
            Author = "Author 2",
            Pages = 600,
            ISBN = new Isbn("56756-a798454"),
            CopiesCount = 10
        };
        var borrower1 = new Borrower { Name = "John Doe" };
        var borrower2 = new Borrower { Name = "Jane Doe" };

        DbContext.Books.AddRange(book1, book2);
        DbContext.Borrowers.AddRange(borrower1, borrower2);

        DbContext.BorrowedBooks.AddRange(
            new BorrowedBook
            {
                BookId = book1.Id,
                BorrowerId = borrower1.Id,
                BorrowedDate = DateTime.Now.AddDays(-10),
                ReturnedDate = DateTime.Now.AddDays(5),
                Book = book1,
                Borrower = borrower1
            },
            new BorrowedBook
            {
                BookId = book2.Id,
                BorrowerId = borrower2.Id,
                BorrowedDate = DateTime.Now.AddDays(-20),
                ReturnedDate = DateTime.Now.AddDays(5),
                Book = book2,
                Borrower = borrower2
            }
        );

        DbContext.SaveChangesAsync().Wait();
    }

    public void Dispose()
    {
        _mssqlContainer.StopAsync().Wait();
    }
}