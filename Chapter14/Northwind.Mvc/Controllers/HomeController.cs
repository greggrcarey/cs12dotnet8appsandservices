using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // To use Include method.
using Northwind.EntityModels; // To use Northwind entity models.
using Northwind.Mvc.Models;
using System.Diagnostics;

namespace Northwind.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext _db;

        public HomeController(ILogger<HomeController> logger, NorthwindContext northwindContext)
        {
            _logger = logger;
            _db = northwindContext;
        }

        public IActionResult Index()
        {
            IEnumerable<Order> model = _db.Orders
              .Include(order => order.Customer)
              .Include(order => order.OrderDetails)
              .OrderByDescending(order => order.OrderDetails
                .Sum(detail => detail.Quantity * detail.UnitPrice))
              .AsEnumerable();
            return View(model);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
