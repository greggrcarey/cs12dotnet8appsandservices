using Northwind.GraphQL.Service;
using Northwind.EntityModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNorthwindContext();

builder.Services
    .AddGraphQLServer()
    .AddFiltering()
    .AddSorting()
    .RegisterDbContext<NorthwindContext>()
    .AddQueryType<Query>();


var app = builder.Build();

app.MapGet("/", () => "Navigate to https://localhost:5121/grapql");
app.MapGraphQL();

app.Run();
