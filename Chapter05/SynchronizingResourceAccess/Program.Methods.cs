partial class Program
{
    private static void MethodA()
    {
        //Checking the lock object is voluntary, but it must be done by each method that touches the shared resource
        /*
         * Directly locking an object like this can cause Deadlocks
         * Using the Monitor class directly gives a way to break out of deadlocks with a timeout
        lock (SharedObjects.LockObject) 
        {
            for (int i = 0; i < 5; i++)
            {
                // Simulate two seconds of work on the current thread.
                Thread.Sleep(Random.Shared.Next(2000));
                // Concatenate the letter "A" to the shared message.
                SharedObjects.Message += "A";
                // Show some activity in the console output.
                Write(".");
            }
        }
        */

        try
        {
            if (Monitor.TryEnter(SharedObjects.LockObject, TimeSpan.FromSeconds(15)))
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(Random.Shared.Next(2000));
                    SharedObjects.Message += "A";
                    Interlocked.Increment(ref SharedObjects.Counter);
                    Write(".");
                }
            }
            else
            {
                WriteLine("Method B timed out when entering a monitor on conch.");
            }
        }
        finally
        {
            Monitor.Exit(SharedObjects.LockObject);
        }



    }
    private static void MethodB()
    {
        /*
        lock (SharedObjects.LockObject)
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(Random.Shared.Next(2000));
                SharedObjects.Message += "B";
                Write(".");
            }
        }
        */

        try
        {
            if (Monitor.TryEnter(SharedObjects.LockObject, TimeSpan.FromSeconds(15)))
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(Random.Shared.Next(2000));
                    SharedObjects.Message += "B";
                    Interlocked.Increment(ref SharedObjects.Counter);
                    Write(".");
                }
            }
            else
            {
                WriteLine("Method B timed out when entering a monitor on conch.");
            }
        }
        finally
        {
            Monitor.Exit(SharedObjects.LockObject);
        }

    }
}
