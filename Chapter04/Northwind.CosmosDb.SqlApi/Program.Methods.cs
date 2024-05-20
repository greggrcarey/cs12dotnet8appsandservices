using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Numerics;




partial class Program
{
    static async Task CreateCosmosResources()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();
        SectionTitle("Creating Cosmos resource");

        try
        {
            using CosmosClient client = new(accountEndpoint: config["endpointUri"],
                authKeyOrResourceToken: config["primaryEndpoint"]);
            {

                DatabaseResponse dbResponse = await client.CreateDatabaseIfNotExistsAsync("Northwind", throughput: 400);

                string status = dbResponse.StatusCode switch
                {
                    HttpStatusCode.OK => "exists",
                    HttpStatusCode.Created => "created",
                    _ => "unknown"
                };

                WriteLine("Database Id: {0}, Status: {1}", arg0: dbResponse.Database.Id, arg1: status);

                IndexingPolicy indexingPolicy = new()
                {
                    IndexingMode = IndexingMode.Consistent,
                    Automatic = true,
                    IncludedPaths = { new IncludedPath { Path = "/*" } }
                };

                ContainerProperties containerProperties = new("Products", partitionKeyPath: "/productId")
                {
                    IndexingPolicy = indexingPolicy,
                };

                ContainerResponse containerResponse = await dbResponse.Database
                    .CreateContainerIfNotExistsAsync(containerProperties, throughput: 1000 /* RU/s */);


                status = dbResponse.StatusCode switch
                {
                    HttpStatusCode.OK => "exists",
                    HttpStatusCode.Created => "created",
                    _ => "unknown"
                };

                WriteLine($"Container Id: {containerResponse.Container.Id}, Status: {status}");

                var container = containerResponse.Container;

                ContainerProperties properties = await container.ReadContainerAsync();

                WriteLine($" PartitionKeyPath: {properties.PartitionKeyPath}");
                WriteLine($" LastModified: {properties.LastModified}");
                WriteLine($" IndexingPolicy.IndexingMode: {properties.IndexingPolicy.IndexingMode}");
                WriteLine(" IndexingPolicy.IncludesPaths: {0}",
                    arg0: string.Join(", ", properties.IndexingPolicy.IncludedPaths.Select(path => path.Path)));
                WriteLine($" IndexingPolicy: {properties.IndexingPolicy}");
            }
        }
        catch (HttpRequestException ex)
        {
            WriteLine($"Error: {ex.Message}");
            WriteLine("Hint: If you are using the Azure Cosmos Emulator then please make sure that it is running.");
        }
        catch (Exception ex)
        {
            WriteLine("Error: {0} says {1}",
            arg0: ex.GetType(),
            arg1: ex.Message);
        }

    }

}