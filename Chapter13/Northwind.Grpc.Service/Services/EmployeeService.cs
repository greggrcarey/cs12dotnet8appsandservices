using Grpc.Core; //ServerCallContext
using Microsoft.Data.SqlClient;
using System.Data; //CommandType
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;

namespace Northwind.Grpc.Service.Services;

//rpc GetEmployee(EmployeeRequest) returns(EmployeeReply);
//rpc GetEmployees(EmployeesRequest) returns(EmployeesReply);
public class EmployeeService : Employee.EmployeeBase
{
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(ILogger<EmployeeService> logger)
    {
        _logger = logger;
    }

    public override async Task<EmployeeReply?> GetEmployee(EmployeeRequest request, ServerCallContext context)
    {
        _logger.LogCritical($"This request has a deadline of {context.Deadline:T}. It is now {DateTime.UtcNow:T}.");

        SqlCommand command = await GetSqlCommand();
        command.CommandText = """
            SELECT EmployeeId, LastName, FirstName, Title, TitleOfCourtesy, BirthDate, HireDate, [Address], 
            City, Region, PostalCode, Country, HomePhone, Extension, Photo, Notes, ReportsTo, PhotoPath
            FROM Employees 
            WHERE EmployeeId = @id
            """;
        command.Parameters.AddWithValue("id", request.EmployeeId);
        var r = await command.ExecuteReaderAsync(CommandBehavior.SingleResult);

        EmployeeReply? reply = null;

        while (await r.ReadAsync())
        {
            reply = SqlDataReaderToEmployee(r);
        }
        await r.CloseAsync();
        return reply;

    }
    public override async Task<EmployeesReply?> GetEmployees(EmployeesRequest request, ServerCallContext context)
    {
        _logger.LogCritical($"This request has a deadline of {context.Deadline:T}. It is now {DateTime.UtcNow:T}.");

        SqlCommand command = await GetSqlCommand();
        command.CommandText = """
            SELECT EmployeeId, LastName, FirstName, Title, TitleOfCourtesy, BirthDate, HireDate, [Address], 
            City, Region, PostalCode, Country, HomePhone, Extension, Photo, Notes, ReportsTo, PhotoPath
            FROM Employees 
            """;
        var r = await command.ExecuteReaderAsync(CommandBehavior.Default);

        EmployeesReply employeesReply = new();
        EmployeeReply? employee = null;

        while (await r.ReadAsync()) 
        {
            employee = SqlDataReaderToEmployee(r);

            employeesReply.Employees.Add(employee);
        }
        await r.CloseAsync();
        return employeesReply;
    }

    private EmployeeReply SqlDataReaderToEmployee(SqlDataReader r)
    {
        return new EmployeeReply
        {
            EmployeeId = r.GetInt32("EmployeeId"),
            LastName = r.GetString("LastName"),
            FirstName = r.GetString("FirstName"),
            Title = r.IsDBNull("Title") ? String.Empty : r.GetString("Title"),
            TitleOfCourtesy = r.IsDBNull("TitleOfCourtesy") ? String.Empty : r.GetString("TitleOfCourtesy"),
            BirthDate = Timestamp.FromDateTime(r.GetDateTime("BirthDate").ToUniversalTime()),
            HireDate = Timestamp.FromDateTime(r.GetDateTime("HireDate").ToUniversalTime()),
            Address = r.IsDBNull("Address") ? String.Empty : r.GetString("Address"),
            City = r.IsDBNull("City") ? String.Empty : r.GetString("City"),
            Region = r.IsDBNull("Region") ? String.Empty : r.GetString("Region"),
            PostalCode = r.IsDBNull("PostalCode") ? String.Empty : r.GetString("PostalCode"),
            Country = r.IsDBNull("Country") ? String.Empty : r.GetString("Country"),
            HomePhone = r.IsDBNull("HomePhone") ? String.Empty : r.GetString("HomePhone"),
            Extension = r.IsDBNull("Extension") ? String.Empty : r.GetString("Extension"),
            Photo = r.IsDBNull("Photo") ? ByteString.Empty : ByteString.FromStream(r.GetStream("Photo")),
            Notes = r.IsDBNull("Notes") ? String.Empty : r.GetString("Notes"),
            ReportsTo = r.IsDBNull("ReportsTo") ? 0 : r.GetInt32("ReportsTo"),
            PhotoPath = r.IsDBNull("PhotoPath") ? String.Empty : r.GetString("PhotoPath")
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
        SqlCommand cmd = connection.CreateCommand();
        cmd.CommandType = CommandType.Text;

        return cmd;
    }
}
