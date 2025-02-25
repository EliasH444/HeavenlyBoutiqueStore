using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using learning.Data;
using learning.Models;
using learning.Services;

namespace learning.Controllers
{
    public class BasketsController : Controller
    {
        private readonly learningContext _context;

        public BasketsController(learningContext context)
        {
            _context = context;
        }

        // View basket
        public async Task<IActionResult> ViewBasket()
        {
            var userId = _userService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var basket = await _context.Baskets
                .Include(b => b.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b => b.UserId == userId);

            return View(basket);
        }
        // Add product to basket
        public async Task<IActionResult> AddToBasket(int productId, int quantity)
        {
            var userId = _userService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var product = await _context.Products.FindAsync(productId);
            if (product == null || quantity <= 0 || product.StockQuantity < quantity)
                return BadRequest("Invalid product or insufficient stock.");

            var basket = await _context.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (basket == null)
            {
                basket = new Basket { UserId = userId };
                _context.Baskets.Add(basket);
            }

            var basketItem = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (basketItem != null)
            {
                basketItem.Quantity += quantity;
            }
            else
            {
                basket.Items.Add(new BasketItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            product.StockQuantity -= quantity;
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewBasket");
        }

        // Remove item from basket
        public async Task<IActionResult> RemoveFromBasket(int basketItemId)
        {
            var userId = _userService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var basketItem = await _context.BasketItems
                .Include(i => i.Basket)
                .FirstOrDefaultAsync(i => i.BasketItemId == basketItemId && i.Basket.UserId == userId);

            if (basketItem == null)
                return NotFound();

            _context.BasketItems.Remove(basketItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewBasket");
        }




        private bool BasketExists(int id)
        {
            return _context.Basket.Any(e => e.BasketID == id);
        }
    }
}
