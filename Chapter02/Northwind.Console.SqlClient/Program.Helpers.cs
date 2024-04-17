using Microsoft.Data.SqlClient;
using System.Collections;
using System.Globalization;
using System.Text.Json.Serialization;

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

    private static void OutputStatistics(SqlConnection connection)
    {
        string[] includeKeys =
        [
            "BytesSent", "BytesRecieved", "ConnectionTime", "SelectRows"
        ];

        IDictionary staticstics = connection.RetrieveStatistics();

        foreach (object? key in staticstics.Keys)
        {
            if(includeKeys.Length == 0 || includeKeys.Contains(key))
            {
                if (int.TryParse(staticstics[key]?.ToString(), out int value))
                {
                    WriteLineInColor($"{key}: {value:N0}", ConsoleColor.Cyan);
                }
            }
        }

    }

}