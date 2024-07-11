namespace Library.Reporting.Models;

public class BorrowedBook
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid BorrowerId { get; set; }
    public DateTime BorrowedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }

    // Navigation props
    public required Book Book { get; set; }
    public required Borrower Borrower { get; set; }
}