using Microsoft.AspNetCore.Mvc;

namespace learning.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
