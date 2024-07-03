using Microsoft.AspNetCore.Mvc;
using Northwind.WebApi.Client.Mvc.Models;
using System.Diagnostics;
using Northwind.EntityModels;

namespace Northwind.WebApi.Client.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("home/products/{name?}")]
    public async Task<IActionResult> Products(string? name = "cha")
    {
        HomeProductsViewModel model = new();

        HttpClient client = _httpClientFactory.CreateClient(name: "Northwind.WebApi.Service");

        model.NameContains = name;
        model.BaseAddress = client.BaseAddress;

        HttpRequestMessage request = new(
            method: HttpMethod.Get,
            requestUri: $"api/products/{name}");

        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            model.Products = await response.Content
            .ReadFromJsonAsync<IEnumerable<Product>>();
        }
        else
        {
            model.Products = [];

            string content = await response.Content.ReadAsStringAsync();

            // Use the range operator .. to start from zero and
            // go to the first carriage return.
            string exceptionMessage = content[..content.IndexOf("\r")];
            model.ErrorMessage = string.Format("{0}: {1}:",
            response.ReasonPhrase, exceptionMessage);
        }
        return View(model);
    }

}

