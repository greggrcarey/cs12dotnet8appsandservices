using Northwind.EntityModels;

namespace Northwind.Common.EntityModels.Tests
{
    public class NorthwindEntityModelTests
    {
        [Fact]
        public void CanConnectIsTrue()
        {
            using NorthwindContext db = new ();//Arrange

            bool canConnect = db.Database.CanConnect();//Act

            Assert.True(canConnect);//Assert
        }
        [Fact]
        public void ProviderIsSqlServer()
        {
            using NorthwindContext db = new ();

            string? provider = db.Database.ProviderName;

            Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", provider);
        }
        [Fact]
        public void ProductId1IsChai()
        {
            using NorthwindContext db = new ();

            var firstProduct = db.Products.Single(p => p.ProductId == 1);

            Assert.Equal("Chai", firstProduct.ProductName);

        }
        [Fact]
        public void EmployeeHasLastRefreshedIn10sWindow()
        {
            using (NorthwindContext db = new())
            {
                Employee employee1 = db.Employees.Single(p => p.EmployeeId == 1);
                DateTimeOffset now = DateTimeOffset.UtcNow;
                Assert.InRange(actual: employee1.LastRefreshed,
                  low: now.Subtract(TimeSpan.FromSeconds(5)),
                  high: now.AddSeconds(5));
            }
        }
    }
}