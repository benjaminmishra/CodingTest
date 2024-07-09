using System.Data;

namespace Library.ReportingService.Models;

public class Book
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required int Pages { get; set; }
    public required Isbn ISBN { get; set; }
    public required int CopiesCount { get; set; }

    // Navigation prop
    private readonly List<BorrowedBook> _borrowedCopies = [];
    public IEnumerable<BorrowedBook> BorrowedCopies => _borrowedCopies;

    public void BorrowCopy(Borrower borrower) 
    {
         if(_borrowedCopies.Count > CopiesCount)
            throw new DataException($"Cannot borrow any more copies of {Title}");
         
        _borrowedCopies.Add(new BorrowedBook { Book = this, Borrower = borrower });
    }
}