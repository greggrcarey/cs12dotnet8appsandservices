partial class Program
{
    private static void MethodA()
    {
        TaskTitle("Starting Method A...");
        OutputThreadInfo();
        Thread.Sleep(3000); // Simulate three seconds of work.
        TaskTitle("Finished Method A.");
    }
    private static void MethodB()
    {
        TaskTitle("Starting Method B...");
        OutputThreadInfo();
        Thread.Sleep(2000); // Simulate two seconds of work.
        TaskTitle("Finished Method B.");
    }
    private static void MethodC()
    {
        TaskTitle("Starting Method C...");
        OutputThreadInfo();
        Thread.Sleep(1000); // Simulate one second of work.
        TaskTitle("Finished Method C.");
    }
    private static void OuterMethod()
    {
        TaskTitle("Outer method starting...");
        //Task innerTask = Task.Factory.StartNew(InnerMethod); //Not ideal since we want the InnerMethod to complete first before the OuterMethod does
        Task innerTask = Task.Factory.StartNew(InnerMethod, TaskCreationOptions.AttachedToParent);// Better
        TaskTitle("Outer method finished.");
    }
    private static void InnerMethod()
    {
        TaskTitle("Inner method starting...");
        Thread.Sleep(2000);
        TaskTitle("Inner method finished.");
    }

}
