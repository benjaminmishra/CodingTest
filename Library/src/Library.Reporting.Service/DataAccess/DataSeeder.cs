using Library.Reporting.Models;

namespace Library.Reporting.DataAccess;

public static class DataSeeder
{
    public static void SeedBooksData(LibraryDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        dbContext.Database.EnsureCreated();

        if (dbContext.Books.Any() && dbContext.Borrowers.Any() && dbContext.BorrowedBooks.Any())
            return;

        // Create some books
        var book1 = new Book
        {
            Title = "The Great Gatsby",
            Author = "F. Scott Fitzgerald",
            Pages = 218,
            ISBN = new Isbn("9780743273565"),
            CopiesCount = 5
        };

        var book2 = new Book
        {
            Title = "1984",
            Author = "George Orwell",
            Pages = 328,
            ISBN = new Isbn("9780451524935"),
            CopiesCount = 3
        };

        var book3 = new Book
        {
            Title = "To Kill a Mockingbird",
            Author = "Harper Lee",
            Pages = 336,
            ISBN = new Isbn("9780061120084"),
            CopiesCount = 4
        };

        var book4 = new Book
        {
            Title = "Moby Dick",
            Author = "Herman Melville",
            Pages = 720,
            ISBN = new Isbn("9780142437247"),
            CopiesCount = 2
        };

        var book5 = new Book
        {
            Title = "War and Peace",
            Author = "Leo Tolstoy",
            Pages = 1225,
            ISBN = new Isbn("9780199232765"),
            CopiesCount = 3
        };

        // Create some borrowers
        var borrower1 = new Borrower
        {
            Name = "John Doe"
        };

        var borrower2 = new Borrower
        {
            Name = "Jane Smith"
        };

        var borrower3 = new Borrower
        {
            Name = "Alice Johnson"
        };

        var borrower4 = new Borrower
        {
            Name = "Bob Brown"
        };

        // Create some borrowed books
        var borrowedBook1 = new BorrowedBook
        {
            Book = book1,
            Borrower = borrower1,
            BorrowedDate = DateTime.UtcNow.AddDays(-5)
        };

        var borrowedBook2 = new BorrowedBook
        {
            Book = book2,
            Borrower = borrower2,
            BorrowedDate = DateTime.UtcNow.AddDays(-2)
        };

        var borrowedBook3 = new BorrowedBook
        {
            Book = book3,
            Borrower = borrower1,
            BorrowedDate = DateTime.UtcNow.AddDays(-1)
        };

        var borrowedBook4 = new BorrowedBook
        {
            Book = book1,
            Borrower = borrower3,
            BorrowedDate = DateTime.UtcNow.AddDays(-3)
        };

        var borrowedBook5 = new BorrowedBook
        {
            Book = book1,
            Borrower = borrower4,
            BorrowedDate = DateTime.UtcNow.AddDays(-4)
        };

        var borrowedBook6 = new BorrowedBook
        {
            Book = book2,
            Borrower = borrower3,
            BorrowedDate = DateTime.UtcNow.AddDays(-1)
        };

        var borrowedBook7 = new BorrowedBook
        {
            Book = book3,
            Borrower = borrower4,
            BorrowedDate = DateTime.UtcNow.AddDays(-5)
        };

        var borrowedBook8 = new BorrowedBook
        {
            Book = book4,
            Borrower = borrower1,
            BorrowedDate = DateTime.UtcNow.AddDays(-2)
        };

        var borrowedBook9 = new BorrowedBook
        {
            Book = book5,
            Borrower = borrower2,
            BorrowedDate = DateTime.UtcNow.AddDays(-1)
        };

        var borrowedBook10 = new BorrowedBook
        {
            Book = book1,
            Borrower = borrower2,
            BorrowedDate = DateTime.UtcNow.AddDays(-7),
            ReturnedDate = DateTime.UtcNow
        };

        var borrowedBook11 = new BorrowedBook
        {
            Book = book3,
            Borrower = borrower3,
            BorrowedDate = DateTime.UtcNow.AddDays(-6),
            ReturnedDate = DateTime.UtcNow.AddDays(-6)
        };

        var borrowedBook12 = new BorrowedBook
        {
            Book = book5,
            Borrower = borrower4,
            BorrowedDate = DateTime.UtcNow.AddDays(-8),
            ReturnedDate = DateTime.UtcNow.AddMonths(3)
        };

        // Add books to the context
        dbContext.Books.AddRange(book1, book2, book3, book4, book5);

        // Add borrowers to the context
        dbContext.Borrowers.AddRange(borrower1, borrower2, borrower3, borrower4);

        // Add borrowed books to the context
        dbContext.BorrowedBooks.AddRange(borrowedBook1, borrowedBook2, borrowedBook3, borrowedBook4, borrowedBook5, borrowedBook6, borrowedBook7, borrowedBook8, borrowedBook9, borrowedBook10, borrowedBook11, borrowedBook12);

        // Save changes to the database
        dbContext.SaveChanges();
    }
}