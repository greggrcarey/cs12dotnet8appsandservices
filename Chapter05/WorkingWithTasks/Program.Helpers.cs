partial class Program
{
    private static void SectionTitle(string title)
    {
        ConsoleColor previousColor = ForegroundColor;
        ForegroundColor = ConsoleColor.DarkYellow;
        WriteLine($"*** {title}");
        ForegroundColor = previousColor;
    }
    private static void TaskTitle(string title)
    {
        ConsoleColor previousColor = ForegroundColor;
        ForegroundColor = ConsoleColor.Green;
        WriteLine($"{title}");
        ForegroundColor = previousColor;
    }
    private static void OutputThreadInfo()
    {
        Thread t = Thread.CurrentThread;
        ConsoleColor previousColor = ForegroundColor;
        ForegroundColor = ConsoleColor.DarkCyan;
        WriteLine(
          "Thread Id: {0}, Priority: {1}, Background: {2}, Name: {3}",
          t.ManagedThreadId, t.Priority, t.IsBackground, t.Name ?? "null");
        ForegroundColor = previousColor;
    }
    private static decimal CallWebService()
    {
        TaskTitle("Starting call to web service...");
        OutputThreadInfo();
        Thread.Sleep(Random.Shared.Next(2000, 4000));
        TaskTitle("Finished call to web service.");
        return 89.99M;
    }
    private static string CallStoredProcedure(decimal amount)
    {
        TaskTitle("Starting call to stored procedure...");
        OutputThreadInfo();
        Thread.Sleep(Random.Shared.Next(2000, 4000));
        TaskTitle("Finished call to stored procedure.");
        return $"12 products cost more than {amount:C}.";
    }

}
