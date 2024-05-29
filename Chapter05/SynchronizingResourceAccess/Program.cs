using System.Diagnostics; // To use Stopwatch.
WriteLine("Please wait for the tasks to complete.");
Stopwatch watch = Stopwatch.StartNew();
Task a = Task.Factory.StartNew(MethodA);
Task b = Task.Factory.StartNew(MethodB);

Task.WaitAll(new Task[] { a, b });
WriteLine();
WriteLine($"Results: {SharedObjects.Message}.");
WriteLine($"{SharedObjects.Counter} string modifications.");
WriteLine($"{watch.ElapsedMilliseconds:N0} elapsed milliseconds.");
/*
 Result without controlling access to shared resource

Results: ABAABABABB.
6,018 elapsed milliseconds.

Results: ABABABAABB.
7,476 elapsed milliseconds.

Result with lock object

Results: BBBBBAAAAA.
7,711 elapsed milliseconds.

Results: AAAAABBBBB.
8,604 elapsed milliseconds.
 
 */