namespace Starters.Tests;

public class StarterTests
{
    #region Tests for IsBookId2Power

    [Theory]
    [InlineData(4)]
    [InlineData(16)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(128)]
    [InlineData(256)]
    public void IsBookId2Power_2PowerNumber_ReturnsTrue(int number)
    {
        var result = Starters.IsBookId2Power(number);

        Assert.True(result);
    }

    [Fact]
    public void IsBookId2Power_Zero_ReturnsFalse()
    {
        var result = Starters.IsBookId2Power(0);

        Assert.False(result);
    }

    [Fact]
    public void IsBookId2Power_MaxUnsignedInteger_ReturnsFalse()
    {
        var result = Starters.IsBookId2Power(int.MaxValue);

        Assert.False(result);
    }

    #endregion

    #region Tests for ReverseBookTitle

    [Theory]
    [InlineData("Hello World", "dlroW olleH")]
    [InlineData("Moby Dick", "kciD yboM")]
    public void ReverseBookTitle_NormalString_ReturnsReversedString(string bookTitleToReverse, string expectedResult)
    {
        var result = Starters.ReverseBookTitle(bookTitleToReverse);

        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void ReverseBookTitle_EmptyString_EmptyString()
    {
        var result = Starters.ReverseBookTitle(string.Empty);
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ReverseBookTitle_SingleCharacter_SameCharacter()
    {
        var input = "a";

        var result = Starters.ReverseBookTitle(input);

        Assert.Equal("a", result);
    }

    [Fact]
    public void ReverseBookTitle_StringWithSpaces_ReversedString()
    {
        var input = "  spaced  ";

        var result = Starters.ReverseBookTitle(input);

        Assert.Equal("  decaps  ", result);
    }

    [Fact]
    public void ReverseBookTitle_StringWithSpecialCharacters_ReversedString()
    {
        var input = "!@# $%^ &*()";

        var result = Starters.ReverseBookTitle(input);

        Assert.Equal(")(*& ^%$ #@!", result);
    }

    [Fact]
    public void ReverseString_PalindromeString_SameString()
    {
        var input = "madam";

        var result = Starters.ReverseBookTitle(input);

        Assert.Equal("madam", result);
    }

    [Fact]
    public void ReverseString_NumericString_ReversedString()
    {
        var input = "1234567890";

        var result = Starters.ReverseBookTitle(input);

        Assert.Equal("0987654321", result);
    }

    [Fact]
    public void ReverseString_MixedCaseString_ReversedString()
    {
        var input = "AbCdEfG";

        var result = Starters.ReverseBookTitle(input);

        Assert.Equal("GfEdCbA", result);
    }

    #endregion

    #region Tests for ReplicateBookTitle

    [Fact]
    public void ReplicateBookTitle_EmptyStringZeroTimes_EmptyString()
    {
        var input = string.Empty;
        var times = 0;

        var result = Starters.ReplicateBookTitle(input, times);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ReplicateBookTitle_EmptyStringMultipleTimes_EmptyString()
    {
        var input = string.Empty;
        var times = 5;

        var result = Starters.ReplicateBookTitle(input, times);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ReplicateBookTitle_NormalStringZeroTimes_EmptyString()
    {
        var input = "Hello";
        var times = 0;

        var result = Starters.ReplicateBookTitle(input, times);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ReplicateBookTitle_NormalStringMultipleTimes_ReplicatedString()
    {
        var input = "Hello";
        var times = 3;

        var result = Starters.ReplicateBookTitle(input, times);

        Assert.Equal("HelloHelloHello", result);
    }

    [Fact]
    public void ReplicateBookTitle_StringWithSpaces_ReplicatedString()
    {
        var input = " Hello ";
        var times = 2;

        var result = Starters.ReplicateBookTitle(input, times);

        Assert.Equal(" Hello  Hello ", result);
    }

    [Fact]
    public void ReplicateBookTitle_StringWithSpecialCharacters_ReplicatedString()
    {
        var input = "!@#";
        var times = 4;

        var result = Starters.ReplicateBookTitle(input, times);

        Assert.Equal("!@#!@#!@#!@#", result);
    }

    [Fact]
    public void ReplicateBookTitle_NumericString_ReplicatedString()
    {
        var input = "123";
        var times = 3;

        var result = Starters.ReplicateBookTitle(input, times);

        Assert.Equal("123123123", result);
    }

    [Fact]
    public void ReplicateBookTitle_SingleCharacterMultipleTimes_ReplicatedString()
    {
        var input = "a";
        var times = 5;

        var result = Starters.ReplicateBookTitle(input, times);

        Assert.Equal("aaaaa", result);
    }

    [Fact]
    public void ReplicateBookTitle_SingleCharacterZeroTimes_EmptyString()
    {
        var input = "a";
        var times = 0;

        var result = Starters.ReplicateBookTitle(input, times);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ReplicateBookTitle_NegetiveNumber_ThrowsArgumentException()
    {
        var input = "a";
        var times = -10;

        Assert.Throws<ArgumentException>(() => Starters.ReplicateBookTitle(input, times));
    }

    #endregion

    #region Tests for PrintOddNumberFromZeroToHundred

    [Fact]
    public void PrintOddNumberFromZeroToHundred_FirstNumberIsOne()
    {
        var output = TestHelpers.CaptureConsoleOutput(Starters.PrintOddNumberFromZeroToHundred);

        var firstNumber = output.Split(Environment.NewLine, StringSplitOptions.TrimEntries)[0];

        Assert.Equal("1", firstNumber);
    }

    [Fact]
    public void PrintOddNumberFromZeroToHundred_LastElementIs99()
    {
        var output = TestHelpers.CaptureConsoleOutput(Starters.PrintOddNumberFromZeroToHundred);

        var lastNumber = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

        Assert.NotNull(lastNumber);
        Assert.Equal("99", lastNumber);
    }

    [Fact]
    public void PrintOddNumberFromZeroToHundred_ContainsCorrectNumberOfElements()
    {
        var output = TestHelpers.CaptureConsoleOutput(Starters.PrintOddNumberFromZeroToHundred);

        var outputCount = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length;

        Assert.Equal(50, outputCount);
    }

    [Fact]
    public void PrintOddNumberFromZeroToHundred_AllElementsAreOdd()
    {
        var output = TestHelpers.CaptureConsoleOutput(Starters.PrintOddNumberFromZeroToHundred);

        var outputLines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.NotEmpty(outputLines);
        // check for all items in the list if they have a remainder after dividing by 2
        Assert.All(
            outputLines,
            x => Assert.True(int.Parse(x.Trim()) % 2 is not 0)
        );
    }

    [Fact]
    public void PrintOddNumberFromZeroToHundred_ContainsExpectedOddNumbers()
    {
        var output = TestHelpers.CaptureConsoleOutput(Starters.PrintOddNumberFromZeroToHundred);

        var outputLines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.NotEmpty(outputLines);
        for (var i = 0; i < outputLines.Length; i++)
        {
            Assert.Equal((2 * i + 1).ToString(), outputLines[i].Trim());
        }
    }

    #endregion
}