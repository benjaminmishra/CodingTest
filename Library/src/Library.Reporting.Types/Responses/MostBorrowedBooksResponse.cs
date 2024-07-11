namespace Library.Reporting.Types;

public record MostBorrowedBooksResponse(IEnumerable<MostBorrowedBook> Books);

public record MostBorrowedBook(string Name, string ISBN, string Author, int TimeBorrowed);