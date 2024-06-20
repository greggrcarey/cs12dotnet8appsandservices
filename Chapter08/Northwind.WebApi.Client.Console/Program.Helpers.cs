partial class Program
{
    private static void WriteInColor(string text, ConsoleColor foregroundColor)
    {
        ConsoleColor previousColor = ForegroundColor;
        ForegroundColor = foregroundColor;
        Write(text);
        ForegroundColor = previousColor;
    }
}
