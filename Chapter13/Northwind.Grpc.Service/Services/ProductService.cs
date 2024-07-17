using Grpc.Core; // To use ServerCallContext.
using Microsoft.Data.SqlClient; //SqlConnection etc
using System.Data;

namespace Northwind.Grpc.Service.Services;
public class ProductService : Product.ProductBase
{
    private readonly ILogger<ProductService> _logger;
    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
    }

    public override async Task<ProductReply?> GetProduct(ProductRequest request, ServerCallContext context)
    {
        _logger.LogCritical("This request has a deadline of {0:T}. It is now {1:T}.", context.Deadline, DateTime.UtcNow);

        var command = await GetSqlCommand();
        command.CommandText = """
            SELECT ProductId, ProductName, SupplierId, CategoryId, QuantityPerUnit, UnitPrice, 
                   UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued 
            FROM Products 
            WHERE ProductId = @id
            """;
        command.Parameters.AddWithValue("id", request.ProductId);
        SqlDataReader r = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

        ProductReply? product = null;

        while (await r.ReadAsync())
        {
            product = SqlReaderToProduct(r);
        }
        await r.CloseAsync();
        return product;
    }

    public override async Task<ProductsReply?> GetProducts(ProductsRequest request, ServerCallContext context)
    {
        _logger.LogCritical("This request has a deadline of {0:T}. It is now {1:T}.", context.Deadline, DateTime.UtcNow);

        var command = await GetSqlCommand();

        command.CommandText = """
            SELECT ProductId, ProductName, SupplierId, CategoryId, QuantityPerUnit, UnitPrice, 
                   UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued 
            FROM Products 
            """;

        ProductsReply? productsReply = new();
        ProductReply? productReply = null;

        SqlDataReader r = command.ExecuteReader(CommandBehavior.Default);
        while (await r.ReadAsync())
        {
            productReply = SqlReaderToProduct(r);

            productsReply.Products.Add(productReply);
        }
        await r.CloseAsync();

        return productsReply;

    }

    public override async Task<ProductsReply?> GetProductsMinimumPrice(ProductsMinimumPriceRequest request, ServerCallContext context)
    {
        _logger.LogCritical("This request has a deadline of {0:T}. It is now {1:T}.", context.Deadline, DateTime.UtcNow);

        var command = await GetSqlCommand();

        command.Parameters.AddWithValue("minimumPrice", (decimal)request.MinimumPrice);
        command.CommandText = """
            SELECT ProductId, ProductName, SupplierId, CategoryId, QuantityPerUnit, UnitPrice, 
                   UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued 
            FROM Products 
            WHERE UnitPrice > @minimumPrice
            """;

        ProductsReply? productsReply = new();
        ProductReply? productReply = null;

        SqlDataReader r = command.ExecuteReader();
        while (await r.ReadAsync())
        {
            productReply = SqlReaderToProduct(r);

            productsReply.Products.Add(productReply);
        }
        await r.CloseAsync();
        return productsReply;
    }

    private ProductReply SqlReaderToProduct(SqlDataReader r)
    {
        return new ProductReply
        {
            ProductId = r.GetInt32("ProductId"),
            ProductName = r.GetString("ProductName"),
            SupplierId = r.GetInt32("SupplierId"),
            CategoryId = r.GetInt32("CategoryId"),
            QuantityPerUnit = r.GetString("QuantityPerUnit"),
            UnitPrice = r.GetDecimal("UnitPrice"),
            UnitsInStock = r.GetInt16("UnitsInStock"),
            UnitsOnOrder = r.GetInt16("UnitsOnOrder"),
            ReorderLevel = r.GetInt16("ReorderLevel"),
            Discontinued = r.GetBoolean("Discontinued")
        };
    }

    private async Task<SqlCommand> GetSqlCommand()
    {
        SqlConnectionStringBuilder builder = new()
        {
            InitialCatalog = "Northwind",
            MultipleActiveResultSets = true,
            Encrypt = true,
            TrustServerCertificate = true,
            ConnectTimeout = 10, // Default is 30 seconds.
            DataSource = @"localhost\SQLEXPRESS", // To use local SQL Server.
            IntegratedSecurity = true
        };

        SqlConnection connection = new(builder.ConnectionString);
        await connection.OpenAsync();
        SqlCommand command = connection.CreateCommand();
        command.CommandType = CommandType.Text;

        return command;
    }
}
