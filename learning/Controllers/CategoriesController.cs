using learning.Data;
using learning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learning.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly learningContext _context;

        public CategoriesController(learningContext context)
        {
            _context = context;
        }

        // GET: /Categories
        public IActionResult Index()
        {
            var categories = _context.Category.ToList();
            return View(categories);
        }

        // GET: /Categories/Details/5
        public IActionResult Details(int id)
        {
            var category = _context.Category
                .Include(c => c.Products)
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}
