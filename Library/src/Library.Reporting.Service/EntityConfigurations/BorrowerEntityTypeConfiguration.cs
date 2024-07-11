using Library.Reporting.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Reporting.EntityConfigurations;

public class BorrowerEntityTypeConfiguration : IEntityTypeConfiguration<Borrower>
{
    public void Configure(EntityTypeBuilder<Borrower> builder)
    {
        builder.ToTable("Borrowers");

        builder.HasKey(x => x.Id).IsClustered();

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x=>x.Name).IsRequired();

        builder
        .HasMany<BorrowedBook>()
        .WithOne(x=>x.Borrower)
        .HasForeignKey(x=>x.BorrowerId);
    }
}
