using Microsoft.AspNetCore.Mvc;

namespace learning.Controllers
{
    public class OrdersController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
