using System.Globalization; // To use CultureInfo.


ConfigureConsole("en-GB"); // Defaults to en-US culture.

SectionTitle("Specifying date and time values");

WriteLine($"DateTime.MinValue: {DateTime.MinValue}");
WriteLine($"DateTime.MaxValue: {DateTime.MaxValue}");
WriteLine($"DateTime.UnixEpoch: {DateTime.UnixEpoch}");
WriteLine($"DateTime.Now: {DateTime.Now}");
WriteLine($"DateTime.Today: {DateTime.Today}");
WriteLine($"DateTime.Today: {DateTime.Today:d}");
WriteLine($"DateTime.Today: {DateTime.Today:D}");

WriteLine();

DateTime xmas = new(year: 2024, month: 12, day: 25);
WriteLine($"Christmas (default format): {xmas}");
WriteLine($"Christmas (custom short format): {xmas:ddd d/M/yy}");
WriteLine($"Christmas (custom long format): {xmas:dddd, dd MMMM yyyy}");
WriteLine($"Christmas (standard long format): {xmas:D}");
WriteLine($"Christmas (sortable): {xmas:u}");
WriteLine($"Christmas is in month {xmas.Month} of the year.");
WriteLine($"Christmas is day {xmas.DayOfYear} of {xmas.Year}.");
WriteLine($"Christmas {xmas.Year} is on a {xmas.DayOfWeek}.");

WriteLine();

SectionTitle("Date and time calculations");

DateTime beforeXmas = xmas.Subtract(TimeSpan.FromDays(12));
DateTime afterXmas = xmas.AddDays(12);
WriteLine($"12 days before Christmas: {beforeXmas:d}");
WriteLine($"12 days after Christmas: {afterXmas:d}");
TimeSpan untilXmas = xmas - DateTime.Now;
WriteLine($"Now: {DateTime.Now}");
WriteLine($"There are {untilXmas.Days} days and {untilXmas.Hours} hours until Christmas {xmas.Year}.");
WriteLine($"There are {untilXmas.TotalHours:N0} hours until Christmas {xmas.Year}.");

WriteLine();

SectionTitle("Milli-, micro-, and nanoseconds");
DateTime preciseTime = new(
year: 2022, month: 11, day: 8,
hour: 12, minute: 0, second: 0,
millisecond: 6, microsecond: 999);

WriteLine($"Millisecond: {preciseTime.Millisecond}, Microsecond: {preciseTime.Microsecond}, Nanosecond: {preciseTime.Nanosecond}");

preciseTime = DateTime.UtcNow;
// Nanosecond value will be 0 to 900 in 100 nanosecond increments.
WriteLine($"Millisecond: {preciseTime.Millisecond}, Microsecond: {preciseTime.Microsecond}, Nanosecond: {preciseTime.Nanosecond}");


SectionTitle("Globalization with dates and times");
// Same as Thread.CurrentThread.CurrentCulture.
WriteLine($"Current culture: {CultureInfo.CurrentCulture.Name}");

string textDate = "4 July 2024";
DateTime independenceDay = DateTime.Parse(textDate);
WriteLine($"Text: {textDate}, DateTime: {independenceDay:d MMMM}");

textDate = "7/4/2024";
independenceDay = DateTime.Parse(textDate);
WriteLine($"Text: {textDate}, DateTime: {independenceDay:d MMMM}");

// Explicitly override the current culture by setting a provider.
independenceDay = DateTime.Parse(textDate, provider: CultureInfo.GetCultureInfo("en-US"));

WriteLine($"Text: {textDate}, DateTime: {independenceDay:d MMMM}");

for (int year = 2023; year <= 2028; year++)
{
    Write($"{year} is a leap year: {DateTime.IsLeapYear(year)}. ");
    WriteLine($"There are {DateTime.DaysInMonth(year: year, month: 2)} days in February {year}.");
}
WriteLine($"Is Christmas daylight saving time? {xmas.IsDaylightSavingTime()}");
WriteLine($"Is July 4th daylight saving time? {independenceDay.IsDaylightSavingTime()}");


WriteLine();

SectionTitle("Localizing the DayOfWeek enum");
CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;
// Explicitly set culture to Danish (Denmark).
Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("da-DK");

// DayOfWeek is not localized to Danish.
WriteLine($"Culture: {Thread.CurrentThread.CurrentCulture.NativeName}, DayOfWeek: { DateTime.Now.DayOfWeek}");

// Use dddd format code to get day of the week localized.
WriteLine($"Culture: {Thread.CurrentThread.CurrentCulture.NativeName}, DayOfWeek: {DateTime.Now:dddd}");

// Use GetDayName method to get day of the week localized.
WriteLine($"Culture: {Thread.CurrentThread.CurrentCulture.NativeName}, DayOfWeek: {DateTimeFormatInfo.CurrentInfo.GetDayName(DateTime.Now.DayOfWeek)}");

Thread.CurrentThread.CurrentCulture = previousCulture;