using System.Text;

namespace Starters;

public static class Starters
{
    /// <summary>
    /// Detects whether a book id is Power of 2
    /// </summary>
    /// <param name="bookId">BookId to test</param>
    /// <returns>True if power of 2 otherwise false</returns>
    public static bool IsBookId2Power(int bookId)
    {
        if (bookId < 1)
            return false;

        while (bookId % 2 == 0)
        {
            bookId /= 2;
        }

        return bookId is 1;
    }

    /// <summary>
    /// Reverses a Book Title
    /// </summary>
    /// <param name="s">Book Title</param>
    /// <returns>Reversed version of the input string</returns>
    public static string ReverseBookTitle(string s)
    {
        if (s == string.Empty)
            return string.Empty;

        var charArr = s.AsSpan();
        var reversedStringBuilder = new StringBuilder();

        for (var i = charArr.Length - 1; i >= 0; i--)
        {
            reversedStringBuilder.Append(charArr[i]);
        }

        return reversedStringBuilder.ToString();
    }

    /// <summary>
    /// Replicates a given book title and returns the concatenated result
    /// </summary>
    /// <param name="s">Book Title to replicate</param>
    /// <param name="n">Number of time replication should happen</param>
    /// <returns>Replicated book title</returns>
    /// <exception cref="ArgumentException">Throws if n is less than zerp</exception>
    public static string ReplicateBookTitle(string s, int n)
    {
        switch (n)
        {
            case < 0:
                throw new ArgumentException($"{nameof(n)} cannot be negative");
            case 0:
                return string.Empty;
            case 1:
                return s;
        }

        var replicatedStringBuilder = new StringBuilder();

        for (var i = 0; i < n; i++)
        {
            replicatedStringBuilder.Append(s);
        }

        return replicatedStringBuilder.ToString();
    }

    /// <summary>
    /// Prints all odd numbers between 0 and 100 to the console
    /// </summary>
    public static void PrintOddNumberFromZeroToHundred()
    {
        for (var i = 1; i <= 100; i += 2)
        {
            Console.WriteLine(i);
        }
    }
}
