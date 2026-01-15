using learning.Data;
using learning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace learning.Controllers
{
    public class AdminController : Controller


    {
        public AdminController(learningContext context)
        {
            _context = context;
        }
        private readonly learningContext _context;
        //Temp workaround to access admin page
        //used

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
           var product =  _context.Product.ToList();
            return View(product);
        }
        //used
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Category.ToList();
            return View(new Product());
        }
        //used
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            /*if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Category.ToList();
                return View(product);
            }*/

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //notused
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateProduct([FromBody]Product product) {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

            }
            else { 
            
                Console.WriteLine("Something wrong model Validation");
            }

                
            return RedirectToAction(nameof(Index));
        }

        //dont think used
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProductsList()
        {
            return View(await _context.Product.ToListAsync());
        }

        //used
        [Authorize(Roles = "Admin")]
        [HttpPost]
        
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }
            if (product == null) {
                Console.WriteLine("not found");            
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MinusStock(int? id) {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product != null)
            {
                product.StockQuantity -= 1;
            }
            await _context.SaveChangesAsync();

            //Update later on to redirect to Admin Product List
            return RedirectToAction(nameof(ProductsList));

        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PlusStock(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product != null)
            {
                product.StockQuantity += 1;
            }
            await _context.SaveChangesAsync();
            //Update later on to redirect to Admin Product List
            return RedirectToAction(nameof(ProductsList));
        }

        [Authorize(Roles = "Admin")]
        // GET: Show Create Category page
        public IActionResult CreateCategory()
        {
            return View(new Category());
        }

        // POST: Handle form submission
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // or redirect to a Category List page
        }


        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Category category ) {

            if (!ModelState.IsValid) {
                Console.WriteLine("Category model fucked");
                //return NotFound;
            }

            try
            {
                //var product = await _context.Category.FirstAsync(m => m.CategoryId == category);
                _context.Category.Remove(category);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex) {

                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return Ok(nameof(Index));
        }



    }
}
