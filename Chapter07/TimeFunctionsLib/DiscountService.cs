namespace Northwind.Services;

public class DiscountService
{
    public decimal GetDiscount()
    {
        //This has a dependancy on the current time provied by the system.

        var now = DateTime.UtcNow;

        return now.DayOfWeek switch
        {
            DayOfWeek.Saturday or DayOfWeek.Sunday => 0.2M,
            _ => 0M
        };
    }
}
