using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net;
using Northwind.EntityModels;//NorthwindContext etc.
using Northwind.CosmosDb.Items;//Cosmos Types
using Microsoft.EntityFrameworkCore;//Include Extension Methods



partial class Program
{
    private static readonly IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();


    static async Task CreateCosmosResources()
    {

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

    static async Task CreateProductItems()
    {
        SectionTitle("Creating Product Items");

        double totalCharge = 0.0;

        try
        {
            using CosmosClient client = new(accountEndpoint: config["endpointUri"],
                authKeyOrResourceToken: config["primaryEndpoint"]);
            var container = client.GetContainer(databaseId: "Northwind", containerId: "Products");

            using NorthwindContext db = new();
            if (!db.Database.CanConnect())
            {
                WriteLine($"Cannot connect to the SQL Server database to " +
                    $"read products using the connection string: {db.Database.GetConnectionString()}");
                return;
            }

            ProductCosmos[] products = [.. db.Products

                //related data for embedding
                .Include(p => p.Category)
                .Include(p => p.Supplier)

                //Filter any product with null category or supplier 
                //to avoid null warnings
                .Where(p => (p.Category != null) && (p.Supplier != null))
                .Select(p => new ProductCosmos
                {
                    id = p.ProductId.ToString(),
                    productId = p.ProductId.ToString(),
                    productName = p.ProductName,
                    quantityPerUnit = p.QuantityPerUnit,

                    //If the related category is null, store null
                    //else map the category to the Cosmos model
                    category = p.Category == null ? null :
                    new CategoryCosmos
                    {
                        categoryId = p.Category.CategoryId,
                        categoryName = p.Category.CategoryName,
                        description = p.Category.Description,
                    },
                    supplier = p.Supplier == null ? null :
                    new SupplierCosmos
                    {
                        supplierId = p.Supplier.SupplierId,
                        companyName = p.Supplier.CompanyName,
                        contactTitle = p.Supplier.ContactTitle,
                        address = p.Supplier.Address,
                        city = p.Supplier.City,
                        country = p.Supplier.Country,
                        postalCode = p.Supplier.PostalCode,
                        region = p.Supplier.Region,
                        phone = p.Supplier.Phone,
                        fax = p.Supplier.Fax,
                        homePage = p.Supplier.HomePage,
                    },
                    unitPrice = p.UnitPrice,
                    unitsInStock = p.UnitsInStock,
                    reorderLevel = p.ReorderLevel,
                    unitsOnOrder = p.UnitsOnOrder,
                    discontinued = p.Discontinued,

                })];

            foreach (ProductCosmos product in products)
            {
                try
                {
                    ItemResponse<ProductCosmos> productResponse = await container.ReadItemAsync<ProductCosmos>(
                        id: product.id, new PartitionKey(product.id));

                    WriteLine($"Item with id: {productResponse.Resource.id} exists. " +
                        $"Query consumed {productResponse.RequestCharge} RUs");

                    totalCharge += productResponse.RequestCharge;
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    //Create item if not exists
                    ItemResponse<ProductCosmos> productResponse = await container.CreateItemAsync(product);

                    WriteLine($"Created item with id: {productResponse.Resource.id}. " +
                       $"Insert consumed {productResponse.RequestCharge} RUs");

                    totalCharge += productResponse.RequestCharge;
                }
                catch (Exception ex)
                {
                    WriteLine($"Error: {ex.GetType()} says: {ex.Message}");
                }

            }
        }
        catch (HttpRequestException ex)
        {
            WriteLine($"Error: {ex.Message}");
            WriteLine("Hint: Check that your Cosmos Db Emulator is running");
        }
        catch (Exception ex)
        {
            WriteLine($"Error: {ex.GetType()} says: {ex.Message}");
        }
        WriteLine("Total requests charge: {0:N2} RUs", totalCharge);
    }

    static async Task ListProductItems(string sqlText = "SELECT * FROM c")
    {
        SectionTitle("Listing Product Items");

        try
        {
            using var client = new CosmosClient(accountEndpoint: config["endpointUri"],
                authKeyOrResourceToken: config["primaryEndpoint"]);

            Container container = client.GetContainer(databaseId: "Northwind", containerId: "Products");

            WriteLine($"Running Query {sqlText}");

            QueryDefinition query = new(sqlText);

            using FeedIterator<ProductCosmos> resultsIterator = container.GetItemQueryIterator<ProductCosmos>(query);

            if (!resultsIterator.HasMoreResults)
            {
                WriteLine("No results found");
            }

            while (resultsIterator.HasMoreResults)
            {
                FeedResponse<ProductCosmos> products = await resultsIterator.ReadNextAsync();

                WriteLine($"Status code: {products.StatusCode}, Request Charge: {products.RequestCharge} RUs");

                WriteLine($"{products.Count} products found");

                foreach (ProductCosmos product in products)
                {
                    WriteLine($"id: {product.id}, productName: {product.productName}, unitPrice: {product.unitPrice.ToString()}");

                }
            }
        }
        catch (HttpRequestException ex)
        {
            WriteLine($"Error: {ex.Message}");
            WriteLine("Hint: If you are using the Azure Cosmos Emulator then please make sure it is running.");
        }
        catch (Exception ex)
        {
            WriteLine("Error: {0} says {1}",
            arg0: ex.GetType(),
            arg1: ex.Message);
        }
    }

    static async Task DeleteProductItems()
    {
        SectionTitle("Deleting Product Items");

        double totalCharge = 0.0;

        try
        {
            using CosmosClient client = new(accountEndpoint: config["endpointUri"],
                authKeyOrResourceToken: config["primaryEndpoint"]);

            Container container = client.GetContainer(databaseId: "Northwind", containerId: "Products");

            string sqlText = "SELECT * FROM c";

            WriteLine($"Running SQL text: {sqlText}");

            QueryDefinition query = new(sqlText);

            using FeedIterator<ProductCosmos> resultsIterator = container.GetItemQueryIterator<ProductCosmos>(query);

            while (resultsIterator.HasMoreResults)
            {
                FeedResponse<ProductCosmos> products = await resultsIterator.ReadNextAsync();

                foreach (ProductCosmos product in products)
                {
                    WriteLine($"Delete id: {product.id}, productName: {product.productName}");

                    ItemResponse<ProductCosmos> response = await container.DeleteItemAsync<ProductCosmos>(id: product.id, partitionKey: new(product.id));

                    WriteLine($"Status Code: {response.StatusCode}, Request charge: {response.RequestCharge}");

                    totalCharge += response.RequestCharge;

                }
            }
        }
        catch (HttpRequestException ex)
        {
            WriteLine($"Error: {ex.Message}");
            WriteLine("Hint: If you are using the Azure Cosmos Emulator then please make sure it is running.");
        }
        catch (Exception ex)
        {
            WriteLine("Error: {0} says {1}",
            arg0: ex.GetType(),
            arg1: ex.Message);
        }
        WriteLine("Total requests charge: {0:N2} RUs", totalCharge);
    }

}