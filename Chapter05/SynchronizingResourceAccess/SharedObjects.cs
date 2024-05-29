public static class SharedObjects
{
    public static string? Message; //Shared Resource

    public static object LockObject = new(); //Shared object to lock

    public static int Counter; //Another Shared Resource
}