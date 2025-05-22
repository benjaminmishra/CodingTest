using Library.Reporting.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Reporting.EntityConfigurations;

public class BorrowedBookEntityTypeConfiguration : IEntityTypeConfiguration<BorrowedBook>
{
    public void Configure(EntityTypeBuilder<BorrowedBook> builder)
    {
        builder.ToTable("BorrowedBooks");

        builder.HasKey(x => x.Id).IsClustered();

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.BorrowedDate).IsRequired();

        builder.Property(x => x.ReturnedDate);

        builder
        .HasOne(x => x.Book)
        .WithMany(x => x.BorrowedCopies)
        .HasForeignKey(x => x.BookId)
        .IsRequired();

        builder
        .HasOne(x => x.Borrower)
        .WithMany(x => x.BorrowedBooks)
        .HasForeignKey(x => x.BorrowerId)
        .IsRequired();
    }
}
