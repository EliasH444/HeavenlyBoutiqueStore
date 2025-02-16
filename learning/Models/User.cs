using System.ComponentModel.DataAnnotations;

namespace learning.Models
{
    public class User
    {

        //Item
        //Order number
        /*
         Product Model: To represent wedding dresses, accessories, etc.
        Order Model: For managing customer orders.
        Customer Model: To store customer information.
        Cart Model: To handle items in the shopping cart.
        Payment Model: For transaction and payment details.*/

        [Required]
        [MinLength(2)]
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string shippingAddress { get; set; }
        public string billing_address { get; set; }
        public string? order_history { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();

        public User() { 
        
        }
    }
}
