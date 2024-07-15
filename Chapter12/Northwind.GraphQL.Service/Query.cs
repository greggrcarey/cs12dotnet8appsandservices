namespace Northwind.GraphQL.Service;

public class Query
{
    public string GetGreeting() => "Hello, World!";
    public string GetFarewell() => "Ciao! Ciao!";
    public int RollTheDie() => Random.Shared.Next(1, 7);
}
