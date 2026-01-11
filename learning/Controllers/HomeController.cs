using learning.Data;
using learning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Encodings.Web;

namespace learning.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly learningContext _context;

        //public int id;
        //public String name;

        public HomeController(ILogger<HomeController> logger, learningContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var product = _context.Product.Where(p => p.ProductId == 1008 || p.ProductId == 1009 || p.ProductId == 1010)
                     .ToList();
            return View(product);
        }

        public string Welcome(string name, int id) { 

            return HtmlEncoder.Default.Encode($"Hello my {name}, ID: {id}");
        
        }

        public IActionResult About() { return View(); }
        public IActionResult Contact() { return View(); }

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
