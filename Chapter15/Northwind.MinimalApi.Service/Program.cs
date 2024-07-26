using Microsoft.AspNetCore.Mvc; //[FromServices]
using Microsoft.Extensions.Options;
using Northwind.EntityModels; //AddNorthwindContext
using System.Text.Json.Serialization; //ReferenceHandler
//Alias for the JsonOptions class. Required not to confuse with Microsoft.AspNetCore.Mvc.JsonOptions
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddNorthwindContext();
builder.Services.Configure<HttpJsonOptions>(options =>
{
    // If we do not preserve references then when the JSON serializer
    // encounters a circular reference it will throw an exception.
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("api/employees", ([FromServices] NorthwindContext db) => Results.Json(db.Employees))
  .WithName("GetEmployees")
  .Produces<Employee[]>(StatusCodes.Status200OK);

app.MapGet("api/employees/{id:int}", ([FromServices] NorthwindContext db, [FromRoute] int id) =>
{
    Employee? employee = db.Employees.Find(id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Json(employee);
    }
})
  .WithName("GetEmployeesById")
  .Produces<Employee>(StatusCodes.Status200OK)
  .Produces(StatusCodes.Status404NotFound);

app.MapGet("api/employees/{country}", ([FromServices] NorthwindContext db, [FromRoute] string country) =>
    Results.Json(db.Employees.Where(employee => employee.Country == country)))
  .WithName("GetEmployeesByCountry")
  .Produces<Employee[]>(StatusCodes.Status200OK);

app.MapPost("api/employees", async ([FromBody] Employee employee, [FromServices] NorthwindContext db) =>
{
    db.Employees.Add(employee);
    await db.SaveChangesAsync();
    return Results.Created($"api/employees/{employee.EmployeeId}", employee);
})
  .Produces<Employee>(StatusCodes.Status201Created);

app.Run();