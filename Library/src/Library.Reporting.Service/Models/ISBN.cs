namespace Library.Reporting.Models;

public record Isbn
{
    public string Code {get; init;}
    public Isbn(string isbnCode)
    {
        if(string.IsNullOrWhiteSpace(isbnCode))
            throw new ArgumentException("ISBN Code cannot be null empty or whitespace");

        Code = isbnCode;
    }
}