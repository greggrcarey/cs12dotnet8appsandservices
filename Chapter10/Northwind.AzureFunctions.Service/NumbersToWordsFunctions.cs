using Humanizer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Northwind.AzureFunctions.Service
{
    public class NumbersToWordsFunctions
    {
        private readonly ILogger _logger;

        public NumbersToWordsFunctions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<NumbersToWordsFunctions>();
        }

        [Function(nameof(NumbersToWordsFunctions))]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string? amount = req.Query["amount"];

            HttpResponseData response;

            if (long.TryParse(amount, out long number))
            {
                response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                response.WriteString(number.ToWords());
            }
            else
            {
                response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                response.WriteString($"Failed to parse: {amount}");
            }
            return response;
        }
    }
}
