using System.Text;

namespace Starters;

public static class Starters
{
    /// <summary>
    /// Detects whether a number is 2 Power
    /// </summary>
    /// <param name="number">Number to test</param>
    /// <returns>True if power of 2 otherwise false</returns>
    public static bool IsNumber2Power(int number)
    {
        if (number < 1)
            return false;

        while (number % 2 == 0)
        {
            number /= 2;
        }

        return number is 1;
    }

    /// <summary>
    /// Reverses a string
    /// </summary>
    /// <param name="s">Input string</param>
    /// <returns>Reversed version of the input string</returns>
    public static string ReverseString(string s)
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
    /// Replicates a given string and returns the concatinated result
    /// </summary>
    /// <param name="s">String to replicate</param>
    /// <param name="n">Number of time replication should happen</param>
    /// <returns>Replicated string</returns>
    /// <exception cref="ArgumentException">Throws if n is less than zerp</exception>
    public static string ReplicateString(string s, int n)
    {
        if( n < 0)
            throw new ArgumentException($"{nameof(n)} cannot be negetive");

        if (n is 0)
            return string.Empty;

        if (n is 1)
            return s;

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
        for (int i = 1; i <= 100; i += 2)
        {
            Console.WriteLine(i);
        }
    }
}
