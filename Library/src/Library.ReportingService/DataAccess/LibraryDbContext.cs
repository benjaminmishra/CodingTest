using Microsoft.EntityFrameworkCore;
using Library.ReportingService.Models;
using System.Reflection;
using Library.ReportingService.EntityConfigurations;

namespace Library.ReportingService.DataAccess;

public class LibraryDbContext : DbContext
{
    public virtual DbSet<Book> Books {get;set;}
    public virtual DbSet<Borrower> Borrowers {get; set;}
    public virtual DbSet<BorrowedBook> BorrowedBooks {get; set;}

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) :  base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BorrowerEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BorrowedBookEntityTypeConfiguration());
    }

}
