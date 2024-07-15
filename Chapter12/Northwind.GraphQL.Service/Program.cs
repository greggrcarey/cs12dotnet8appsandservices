using Northwind.GraphQL.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGet("/", () => "Navigate to https://localhost:5121/grapql");
app.MapGraphQL();

app.Run();
