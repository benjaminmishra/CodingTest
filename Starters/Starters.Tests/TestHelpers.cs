namespace Starters.Tests;

public static class TestHelpers
{
    public static string CaptureConsoleOutput(Action action)
    {
        var output = new StringWriter { NewLine = Environment.NewLine };
        Console.SetOut(output);
        action.Invoke();
        return output.ToString();
    }
}
