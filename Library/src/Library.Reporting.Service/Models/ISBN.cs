namespace Library.Reporting.Models;

public record Isbn
{
    public string Code {get; init;}
    public Isbn(string ISBNCode)
    {
        if(string.IsNullOrWhiteSpace(ISBNCode))
            throw new ArgumentException("ISBN Code cannot be null empty or whitespace");

        Code = ISBNCode;
    }
}