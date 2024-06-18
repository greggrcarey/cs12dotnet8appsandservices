using Northwind.EntityModels; // To use the AddNorthwindContext method.
using Packt.Extensions; // To use MapGets and so on.


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddNorthwindContext();
builder.Services.AddCustomHttpLogging();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseHttpLogging();


app.MapGets() // Default pageSize: 10.
  .MapPosts()
  .MapPuts()
  .MapDeletes();

app.Run();


//Creating a web page JavaScript client chapter 8