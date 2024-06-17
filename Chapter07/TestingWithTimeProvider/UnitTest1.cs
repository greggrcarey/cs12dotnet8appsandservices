using Northwind.Services;

namespace TestingWithTimeProvider;

public class TimeTests
{
    [Fact]
    public void TestDiscountDuringWorkDays()
    {
        //Arrange
        DiscountService discountService = new ();

        //Act
        decimal discount = discountService.GetDiscount();

        //Assert
        Assert.Equal(0M, discount);
    }

    [Fact]
    public void TestDiscountDuringWeekends()
    {
        DiscountService discountService = new ();

        decimal discount = discountService.GetDiscount();

        Assert.Equal(0.2M, discount);
    }
}
