using Northwind.EntityModels; //AddNorthwindContext
using Northwind.Blazor.Components;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();//Required for interactivity
builder.Services.AddNorthwindContext();

builder.Services.AddHttpClient(name: "Northwind.Blazor.Service",
  configureClient: options =>
  {
      options.BaseAddress = new("https://localhost:5153/");
      options.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue(
          "application/json", 1.0));
  });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();//Required for interactivity

app.Run();
