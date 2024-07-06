using Microsoft.Extensions.DependencyInjection; //AddHttpClient
using System.Net.Http.Headers;// MediaTypeWithQualityHeaderValue
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddHttpClient(name: "Amazon",
        configureClient: options =>
        {
            options.BaseAddress = new Uri("https://www.amazon.com");
            // Pretend to be Chrome with US English.
            options.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("text/html"));
            options.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            options.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            options.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("image/avif"));
            options.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("image/webp"));
            options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/apng"));
            options.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("*/*", 0.8));
            options.DefaultRequestHeaders.AcceptLanguage.Add(
            new StringWithQualityHeaderValue("en-US"));
            options.DefaultRequestHeaders.AcceptLanguage.Add(
            new StringWithQualityHeaderValue("en", 0.8));
            options.DefaultRequestHeaders.UserAgent.Add(
            new(productName: "Chrome", productVersion: "126.0.6478.127"));
        });
    })
    .Build();

host.Run();
