using Library.ReportingService.DataAccess;
using Library.ReportingService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.ReportingService.EntityConfigurations;

public class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(x => x.Id).IsClustered();

        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Title).IsRequired();

        builder.Property(x => x.Pages).IsRequired();

        builder.Property(x => x.Author).IsRequired();

        builder.Property(x => x.ISBN)
        .HasConversion(x => x.Code, x => new Isbn(x))
        .IsRequired();
    }
}
