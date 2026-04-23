using learning.Data;
using learning.Models;
using learning.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace learning.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly learningContext _context;
        private readonly IImageService _imageService;

        public AdminController(learningContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // ── Index ─────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            // Filtered include — only pulls the primary image, not all images
            var products = await _context.Product
                .Include(p => p.Images.Where(i => i.IsPrimary))
                .ToListAsync();

            return View(products);
        }

        // ── Create GET ────────────────────────────────────────────────────────
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Category.ToList();
            return View(new Product());
        }

        // ── Create POST ───────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile>? images)
        {
            Console.WriteLine("Create Post called ");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("shit");
                ViewBag.Categories = _context.Category.ToList();
                return View(product);
            }

            product.CreatedAt = DateTime.UtcNow;
            product.Images = new List<ProductImage>();

            if (images != null && images.Count > 0)
            {
                bool isFirst = true;

                foreach (var file in images.Where(f => f.Length > 0).Take(10))
                {
                    var url = await _imageService.UploadImageAsync(file, "products");
                    if (url == null) continue;

                    product.Images.Add(new ProductImage
                    {
                        Url = url,
                        IsPrimary = isFirst,
                        DisplayOrder = product.Images.Count,
                        UploadedAt = DateTime.UtcNow
                    });

                    isFirst = false;
                }
            }

            _context.Product.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"'{product.Name}' created with {product.Images.Count} image(s).";
            return RedirectToAction(nameof(Index));
        }

        // ── Edit GET ──────────────────────────────────────────────────────────
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Product
                .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            ViewBag.Categories = _context.Category.ToList();
            return View(product);
        }

        // ── Edit POST ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product vm, List<IFormFile>? images)
        {
            var product = await _context.Product
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Category.ToList();
                return View(product);
            }

            product.Name = vm.Name;
            product.Description = vm.Description;
            product.Price = vm.Price;
            product.StockQuantity = vm.StockQuantity;
            product.CategoryId = vm.CategoryId;

            if (images != null && images.Count > 0)
            {
                int slots = Math.Max(0, 10 - product.Images.Count);

                foreach (var file in images.Where(f => f.Length > 0).Take(slots))
                {
                    var url = await _imageService.UploadImageAsync(file, "products");
                    if (url == null) continue;

                    product.Images.Add(new ProductImage
                    {
                        Url = url,
                        IsPrimary = !product.Images.Any(),
                        DisplayOrder = product.Images.Count,
                        UploadedAt = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"'{product.Name}' updated.";
            return RedirectToAction(nameof(Index));
        }

        // ── Set Primary Image ─────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPrimaryImage(int productId, int imageId)
        {
            var images = await _context.ProductImage
                .Where(i => i.ProductId == productId)
                .ToListAsync();

            if (!images.Any()) return NotFound();

            foreach (var img in images)
                img.IsPrimary = img.ProductImageId == imageId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Primary image updated.";
            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        // ── Delete Image ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId, int productId)
        {
            var image = await _context.ProductImage.FindAsync(imageId);
            if (image == null) return NotFound();

            if (image.IsPrimary)
            {
                var next = await _context.ProductImage
                    .Where(i => i.ProductId == productId && i.ProductImageId != imageId)
                    .OrderBy(i => i.DisplayOrder)
                    .FirstOrDefaultAsync();

                if (next != null) next.IsPrimary = true;
            }

            await _imageService.DeleteImageAsync(image.Url);
            _context.ProductImage.Remove(image);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Image deleted.";
            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        // ── Delete Product ────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var image in product.Images)
                await _imageService.DeleteImageAsync(image.Url);

            _context.ProductImage.RemoveRange(product.Images);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"'{product.Name}' deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ── Stock: Minus ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MinusStock(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null) return NotFound();

            if (product.StockQuantity > 0)
                product.StockQuantity -= 1;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ── Stock: Plus ───────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlusStock(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null) return NotFound();

            product.StockQuantity += 1;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ── Create Category GET ───────────────────────────────────────────────
        public IActionResult CreateCategory() => View(new Category());

        // ── Create Category POST ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);

            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Category '{category.Name}' created.";
            return RedirectToAction(nameof(Index));
        }

        // ── Delete Category ───────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null) return NotFound();

            bool hasProducts = await _context.Product.AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
            {
                TempData["Error"] = "Cannot delete a category that still has products assigned.";
                return RedirectToAction(nameof(Index));
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Category deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}