namespace Library.Reporting.Models;

public class Borrower
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<BorrowedBook> BorrowedBooks { get; set; } = [];
}

