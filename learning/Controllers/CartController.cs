/*using learning.Data;
//using learning.Helpers;
using learning.Models;
using learning.Services;
using Microsoft.AspNetCore.Mvc;


namespace learning.Controllers
{
    public class CartController : Controller
    {
        private readonly learningContext _context;

        public CartController(learningContext context)
        {
            _context = context;
        }

        private List<CartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("cart");
            return cart ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObject("cart", cart);
        }


        // -------------------------------
        //     VIEW CART
        // -------------------------------
        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.Total = cart.Sum(x => x.Price * x.Quantity);
            return View(cart);
        }


        // -------------------------------
        //     ADD TO CART
        // -------------------------------
        public IActionResult Add(int id)
        {
            var product = _context.Product.FirstOrDefault(x => x.ProductId == id);
            if (product == null)
                return NotFound();

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }
            else
            {
                cartItem.Quantity++;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }


        // -------------------------------
        //     REMOVE ITEM
        // -------------------------------
        public IActionResult Remove(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }


        // -------------------------------
        //     UPDATE QUANTITY
        // -------------------------------
        [HttpPost]
        public IActionResult Update(int id, int quantity)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                item.Quantity = quantity < 1 ? 1 : quantity;
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }


        // -------------------------------
        //     CLEAR CART
        // -------------------------------
        public IActionResult Clear()
        {
            SaveCart(new List<CartItem>());
            return RedirectToAction("Index");
        }
    }
}
*/