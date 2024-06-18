using Microsoft.AspNetCore.HttpLogging; // To use HttpLoggingFields.
namespace Packt.Extensions;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCustomHttpLogging(this IServiceCollection services)
    {
        services.AddHttpLogging(options =>
        {
            // Add the Origin header so it will not be redacted.
            options.RequestHeaders.Add("Origin");
            // By default, the response body is not included.
            options.LoggingFields = HttpLoggingFields.All;
        });
        return services;
    }
}
