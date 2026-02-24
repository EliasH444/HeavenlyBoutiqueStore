using learning.Data;
using learning.Models;
using learning.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace learning.Controllers
{
    public class BasketsController : Controller
    {
        private readonly learningContext _context;
        private readonly IUserService _userService;

        public BasketsController(learningContext context, IUserService UserService)
        {
            _context = context;
            _userService = UserService;
        }

        // View basket
        [HttpGet]
        public async Task<IActionResult> ViewBasket()
        {
            var userId = _userService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var basket = await _context.Basket
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

            var product = await _context.Product.FindAsync(productId);
            if (product == null || quantity <= 0 || product.StockQuantity < quantity)
                return BadRequest("Invalid product or insufficient stock.");

            var basket = await _context.Basket
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = userId,
                    Items = new List<BasketItem>()

                };
                _context.Basket.Add(basket);
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

            var basketItem = await _context.BasketItem
                .Include(i => i.Basket)
                .FirstOrDefaultAsync(i => i.BasketItemId == basketItemId && i.Basket.UserId == userId);

            if (basketItem == null)
                return NotFound();

            _context.BasketItem.Remove(basketItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewBasket");
        }

        public async Task<IActionResult> Update (int id, int quantity)
        {
            var basketItem = await _context.BasketItem.FindAsync(id);
            if (basketItem == null)
            {
                return NotFound();
            }
            if (quantity <= 0)
            {
                _context.BasketItem.Remove(basketItem);
            }
            else
            {
                basketItem.Quantity = quantity;
                _context.BasketItem.Update(basketItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewBasket));
        }


        private bool BasketExists(int id)
        {
            return _context.Basket.Any(e => e.BasketID == id);
        }

        //Stripe Section

        // CREATE STRIPE CHECKOUT SESSION
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            Console.WriteLine("Checkout initiated");
            var userId = _userService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var basket = await _context.Basket
                .Include(b => b.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (basket == null || !basket.Items.Any())
                return RedirectToAction("ViewBasket");

            // Map basket items to Stripe line items
            var lineItems = basket.Items.Select(i => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(i.Product.Price * 100), // Stripe expects pence
                    Currency = "gbp",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = i.Product.Name
                    }
                },
                Quantity = i.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = Url.Action("PaymentSuccess", "Baskets", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = Url.Action("ViewBasket", "Baskets", null, Request.Scheme)
            };

            var service = new SessionService();
            var session = service.Create(options);

            // Redirect to Stripe Checkout
            return Redirect(session.Url);
        }

        // PAYMENT SUCCESS
        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(string session_id)
        {
            var userId = _userService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var basket = await _context.Basket
                .Include(b => b.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (basket == null || !basket.Items.Any())
                return RedirectToAction("ViewBasket");

            // Create order from basket
            var purchase = new Purchase
            {
                UserId = userId,
                TotalAmount = basket.Items.Sum(i => i.Product.Price * i.Quantity),
                CreatedAt = DateTime.Now,
                Items = basket.Items.Select(i => new PurchaseItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Product.Price
                }).ToList()
            };

            _context.Purchase.Add(purchase);

            // Clear basket
            _context.BasketItem.RemoveRange(basket.Items);
            await _context.SaveChangesAsync();

            return View(purchase);
        }


    }
}
