using System.Globalization;

partial class Program
{
    private static void ConfigureConsole(string culture = "en-US", bool useComputerCulture  = false)
    {
        OutputEncoding = System.Text.Encoding.UTF8;
        if(!useComputerCulture)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
        }

        WriteLine($"CurrentCulture: {CultureInfo.CurrentCulture.DisplayName}");
    }

    private static void WriteLineInColor(string value, ConsoleColor color = ConsoleColor.White)
    {
        ConsoleColor previousColor = ForegroundColor;
        ForegroundColor = color;
        WriteLine(value);
        ForegroundColor = previousColor;
    }
}