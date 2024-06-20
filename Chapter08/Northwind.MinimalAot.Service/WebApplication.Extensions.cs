using Microsoft.Data.SqlClient; // To use SqlConnection and so on.
using Northwind.Models; // To use Product.
using System.Data; // To use CommandType.
namespace Packt.Extensions;
public static class WebApplicationExtensions
{
    public static WebApplication MapGets(this WebApplication app)
    {
        // app.MapGet(pattern, handler);
        app.MapGet("/", () => "Hello from a native AOT minimal API web service.");
        app.MapGet("/products", GetProducts);
        app.MapGet("/products/{minimumUnitPrice:decimal?}", GetProducts);
        return app;
    }
    private static List<Product> GetProducts(decimal? minimumUnitPrice = null)
    {
        SqlConnectionStringBuilder builder = new()
        {
            InitialCatalog = "Northwind",
            MultipleActiveResultSets = true,
            Encrypt = true,
            TrustServerCertificate = true,
            ConnectTimeout = 10, // Default is 30 seconds.
            DataSource = @"localhost\SQLEXPRESS", // Local SQL Server
            IntegratedSecurity = true
        };
        /*
        // To use SQL Server Authentication:
        builder.UserID = Environment.GetEnvironmentVariable("MY_SQL_USR");
        builder.Password = Environment.GetEnvironmentVariable("MY_SQL_PWD");
        builder.PersistSecurityInfo = false;
        */
        SqlConnection connection = new(builder.ConnectionString);
        connection.Open();
        SqlCommand cmd = connection.CreateCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText =
          "SELECT ProductId, ProductName, UnitPrice FROM Products";
        if (minimumUnitPrice.HasValue)
        {
            cmd.CommandText += " WHERE UnitPrice >= @minimumUnitPrice";
            cmd.Parameters.AddWithValue("minimumUnitPrice", minimumUnitPrice);
        }
        SqlDataReader r = cmd.ExecuteReader();
        List<Product> products = [];
        while (r.Read())
        {
            Product p = new()
            {
                ProductId = r.GetInt32("ProductId"),
                ProductName = r.GetString("ProductName"),
                UnitPrice = r.GetDecimal("UnitPrice")
            };
            products.Add(p);
        }
        r.Close();
        return products;
    }
}
