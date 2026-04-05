using learning.Data;
using learning.Models;
using learning.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
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
        private readonly StripeSettings _stripeSettings;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BasketsController(learningContext context, IUserService UserService, IOptions<StripeSettings> stripeSettings,IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userService = UserService;
            _stripeSettings = stripeSettings.Value;
            _emailService = emailService;
            _userManager = userManager;
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

            // ⚠️ SET THE STRIPE API KEY HERE
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

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
            var session = service.Create(options); // ✅ Now it will work

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

            // ===== SEND ORDER CONFIRMATION EMAIL =====
            ApplicationUser user = await _userManager.GetUserAsync(User);
            var userEmail = await _userManager.GetEmailAsync(user); // Implement this if not already
            if (!string.IsNullOrEmpty(userEmail))
            {
                var subject = "Order Confirmation - Your Store";
                var body = $"Hello {userEmail},<br/><br/>" +
                           $"Thank you for your order! Your order ID is <b>{purchase.PurchaseID}</b>.<br/>" +
                           $"Order Total: £{purchase.TotalAmount:F2}<br/><br/>" +
                           $"Items:<br/>" +
                           string.Join("<br/>", purchase.Items.Select(i => $"{i.ProductName} x{i.Quantity} - £{i.Price * i.Quantity:F2}")) +
                           "<br/><br/>We appreciate your business!";

                await _emailService.SendEmailAsync(userEmail, subject, body);
            }
            return View(purchase);
        }
    }
}
