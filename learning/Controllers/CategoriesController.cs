using learning.Data;
using learning.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace learning.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly learningContext _context;

        public CategoriesController(learningContext context)
        {
            _context = context;
        }
        // GET: CatergoriesController
        public ActionResult Index()
        {
            return View();
        }

     
        // GET: CatergoriesController/Details/5
        public async Task<IActionResult> CollectCategoryID(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return view with validation errors
            }

            var category = await _context.Category.FindAsync(model.CategoryId);
            if (category == null)
            {
                // Handle the case where the category is not found
                ModelState.AddModelError(string.Empty, "Category not found.");
                return View(model);
            }
            return View(category);
        }

        // GET: CatergoriesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CatergoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CatergoriesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CatergoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CatergoriesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CatergoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
