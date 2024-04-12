using static System.Console;

partial class Program
{
    static void WhatsMyNamespace()//This is a static function
    {
        WriteLine($"Namespace of the Program class: {typeof(Program).Namespace ?? "null"}");
    }
}