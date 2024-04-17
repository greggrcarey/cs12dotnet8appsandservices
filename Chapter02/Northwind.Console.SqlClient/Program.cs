using Microsoft.Data.SqlClient; // To use SqlConnection and so on.
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Data;
using System.Formats.Asn1;
using System.Runtime.Serialization; //CommandType
using System.Text.Json; //UTF8JsonWriter, JsonSerializer
using Northwind.Models;
using static System.Environment;
using static System.IO.Path;
using System.ComponentModel;
using Dapper;

ConfigureConsole();

#region Set up the connection string builder

SqlConnectionStringBuilder builder = new()
{
    InitialCatalog = "Northwind",
    MultipleActiveResultSets = true,
    Encrypt = true,
    TrustServerCertificate = true,
    ConnectTimeout = 10 // Default is 30 seconds.
};

WriteLine("Connect to:");
WriteLine("  1 - SQL Server on local machine");
WriteLine("  2 - Azure SQL Database");
WriteLine("  3 – Azure SQL Edge");
WriteLine();
Write("Press a key: ");
ConsoleKey key = ReadKey().Key;
WriteLine();
WriteLine();

switch (key)
{
    case ConsoleKey.D1 or ConsoleKey.NumPad1:
        builder.DataSource = "localhost\\SQLEXPRESS";
        break;
    case ConsoleKey.D2 or ConsoleKey.NumPad2:
        builder.DataSource =
          // Use your Azure SQL Database server name.
          "tcp:apps-services-book.database.windows.net,1433";
        break;
    case ConsoleKey.D3 or ConsoleKey.NumPad3:
        builder.DataSource = "tcp:127.0.0.1,1433";
        break;
    default:
        WriteLine("No data source selected.");
        return;
}

WriteLine("Authenticate using:");
WriteLine("  1 – Windows Integrated Security");
WriteLine("  2 – SQL Login, for example, sa");
WriteLine();
Write("Press a key: ");
key = ReadKey().Key;
WriteLine();
WriteLine();

if (key is ConsoleKey.D1 or ConsoleKey.NumPad1)
{
    builder.IntegratedSecurity = true;
}
else if (key is ConsoleKey.D2 or ConsoleKey.NumPad2)
{
    Write("Enter your SQL Server user ID: ");
    string? userId = ReadLine();
    if (string.IsNullOrWhiteSpace(userId))
    {
        WriteLine("User ID cannot be empty or null.");
        return;
    }
    builder.UserID = userId;
    Write("Enter your SQL Server password: ");
    string? password = ReadLine();
    if (string.IsNullOrWhiteSpace(password))
    {
        WriteLine("Password cannot be empty or null.");
        return;
    }
    builder.Password = password;
    builder.PersistSecurityInfo = false;
}
else
{
    WriteLine("No authentication selected.");
    return;
}
#endregion

#region Create and open the connection
SqlConnection connection = new(builder.ConnectionString);
WriteLine(connection.ConnectionString);
WriteLine();
connection.StateChange += Connection_StateChange;
connection.InfoMessage += Connection_InfoMessage;
try
{
    WriteLine("Opening connection. Please wait up to {0} seconds...",
      builder.ConnectTimeout);
    WriteLine();
    //connection.Open();
    await connection.OpenAsync();
    WriteLine($"SQL Server version: {connection.ServerVersion}");
    connection.StatisticsEnabled = true;
}
catch (SqlException ex)
{
    WriteLineInColor($"SQL exception: {ex.Message}",
      ConsoleColor.Red);
    return;
}
#endregion

#region Collect User Input, Show Output

SqlCommand cmd = connection.CreateCommand();
WriteLine("Execute command uing: ");
WriteLine("     1 - Text");
WriteLine("     2 - Stored Procedure");
WriteLine("Press a key");
key = ReadKey().Key;
WriteLine(); WriteLine();

Write("Enter a unit price: ");
string? priceText = ReadLine();
if (!decimal.TryParse(priceText, out decimal price))
{
    WriteLine("You must enter a valid unitprice.");
    return;
}

SqlParameter p1 = new(), p2 = new(), p3 = new();

if (key is ConsoleKey.D1 or ConsoleKey.NumPad1)
{
    cmd.CommandType = CommandType.Text;
    cmd.CommandText = "SELECT ProductId, ProductName, UnitPrice FROM Products"
        + " WHERE UnitPrice >= @minimumPrice";
    cmd.Parameters.AddWithValue("minimumPrice", price);
}
else if (key is ConsoleKey.D2 or ConsoleKey.NumPad2)
{
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.CommandText = "GetExpensiveProducts";

    p1 = new()
    {
        ParameterName = "price",
        SqlDbType = SqlDbType.Money,
        SqlValue = price
    };
    p2 = new()
    {
        Direction = ParameterDirection.Output,
        ParameterName = "count",
        SqlDbType = SqlDbType.Int
    };
    p3 = new()
    {
        Direction = ParameterDirection.ReturnValue,
        ParameterName = "rv",
        SqlDbType = SqlDbType.Int
    };

    cmd.Parameters.AddRange([p1, p2, p3]);
}

//SqlDataReader reader = cmd.ExecuteReader();
SqlDataReader reader = await cmd.ExecuteReaderAsync();


string jsonPath = Combine(CurrentDirectory, "product.json");
List<Product> products = new(capacity: 77);

while (await reader.ReadAsync())
{
    Product product = new()
    {
        ProductId = await reader.GetFieldValueAsync<int>("ProductId"),
        ProductName = await reader.GetFieldValueAsync<string>("ProductName"),
        UnitPrice = await reader.GetFieldValueAsync<decimal>("UnitPrice")
    };

    products.Add(product);
}




await using (FileStream jsonStream = File.Create(jsonPath))
{
    Utf8JsonWriter utf8JsonWriter = new(jsonStream);
    utf8JsonWriter.WriteStartArray();
    while (await reader.ReadAsync())
    {
        WriteLine("| {0,5} | {1,-35} | {2,10:C} |",
        await reader.GetFieldValueAsync<int>("ProductId"),
        await reader.GetFieldValueAsync<string>("ProductName"),
        await reader.GetFieldValueAsync<decimal>("UnitPrice"));
        
        utf8JsonWriter.WriteStartObject();
        utf8JsonWriter.WriteNumber("productId",await reader.GetFieldValueAsync<int>("ProductId"));
        utf8JsonWriter.WriteString("productName", await reader.GetFieldValueAsync<string>("ProductName"));
        utf8JsonWriter.WriteNumber("unitPrice",await reader.GetFieldValueAsync<decimal>("UnitPrice"));
        utf8JsonWriter.WriteEndObject();
    }
    utf8JsonWriter.WriteEndArray();
    utf8JsonWriter.Flush();
    jsonStream.Close();

}
WriteLineInColor($"File written to: {jsonPath}", ConsoleColor.DarkGreen);


string horizontalLine = new('-', 60);

WriteLine(horizontalLine);
WriteLine("| {0,5} | {1,-35} | {2,10:C}", "Id", "Name", "Price");
WriteLine(horizontalLine);

while (await reader.ReadAsync())
{
    WriteLine("| {0,5} | {1,-35} | {2,10:C}",
    await reader.GetFieldValueAsync<int>("ProductId"),
    await reader.GetFieldValueAsync<string>("ProductName"),
    await reader.GetFieldValueAsync<decimal>("UnitPrice"));
}
WriteLine(horizontalLine);

WriteLineInColor(JsonSerializer.Serialize(products), ConsoleColor.Magenta);
await reader.CloseAsync(); //Closing the reader is required before reading parameters

if (key is ConsoleKey.D2 or ConsoleKey.NumPad2)
{
    WriteLine($"Output count: {p2.Value}");
    WriteLine($"Return value: {p3.Value}");
}

#endregion

OutputStatistics(connection);

#region Dapper
WriteLineInColor("Using Dapper", ConsoleColor.DarkGreen);
connection.ResetStatistics(); //Compare using Dapper

IEnumerable<Supplier> suppliers = connection.Query<Supplier>(
    sql: "SELECT * FROM Suppliers WHERE Country=@Country",
    param: new {Country = "Germany"});

foreach (var item in suppliers)
{
    WriteLine("{0}: {1}, {2}, {3}",
        item.SupplierId, item.CompanyName, item.City, item.Country);
}
WriteLineInColor(JsonSerializer.Serialize(suppliers), ConsoleColor.Green);
OutputStatistics(connection);


IEnumerable<Product> productsFromDapper = connection.Query<Product>(
    sql: "GetExpensiveProducts",
    param: new { price = 100M, count = 0 },
    commandType: CommandType.StoredProcedure);

foreach(var item in productsFromDapper)
{
    WriteLine("{0}: {1}, {2}", item.ProductId, item.ProductName, item.UnitPrice);
}

WriteLineInColor(JsonSerializer.Serialize(productsFromDapper), ConsoleColor.Green);

#endregion


await connection.CloseAsync();
