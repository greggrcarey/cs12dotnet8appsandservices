// See https://aka.ms/new-console-template for more information
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Northwind.EntityModels;
using Northwind.Models;
using System.Data;
using BenchmarkDotNet.Attributes;

namespace Database.Compare;

public class Compare
{
    [Benchmark]
    public void Efcore()
    {
        using NorthwindDb db = new();
        var efProducts = db.Products.ToList();

        WriteLine();
        WriteLine("From efcore");
        WriteLine();
        foreach (var product in efProducts)
        {
            WriteLine($"{product.ProductId}, {product.ProductName}, {product.UnitPrice}");
        }
    }

    [Benchmark]
    public void AdoNet()
    {
        SqlConnectionStringBuilder builder = new()
        {
            DataSource = @"localhost\SQLEXPRESS",
            InitialCatalog = "Northwind",
            TrustServerCertificate = true,
            MultipleActiveResultSets = true,
            // If using Azure SQL Edge.
            // builder.DataSource = "tcp:127.0.0.1,1433";
            // Because we want to fail fast. Default is 15 seconds.
            ConnectTimeout = 3,
            // If using Windows Integrated authentication.
            IntegratedSecurity = true
        };



        SqlConnection sqlConnection = new SqlConnection(builder.ConnectionString);
        sqlConnection.Open();

        var cmd = sqlConnection.CreateCommand();

        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = "SELECT * FROM Products";

        var reader = cmd.ExecuteReader();
        List<Northwind.Models.Product> products = [];

        while (reader.Read())
        {
            Northwind.Models.Product product = new()
            {
                ProductId = reader.GetFieldValue<int>("ProductId"),
                ProductName = reader.GetFieldValue<string>("ProductName"),
                UnitPrice = reader.GetFieldValue<decimal>("UnitPrice")
            };

            products.Add(product);
        }

        WriteLine();
        WriteLine("From ADO");
        WriteLine();
        foreach (var product in products)
        {
            WriteLine($"{product.ProductId}, {product.ProductName}, {product.UnitPrice}");
        }

        sqlConnection.Close();

    }

}






