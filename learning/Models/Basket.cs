namespace learning.Models
{
    public class Basket
    {
        public int BasketID { get; set; }
        public string UserId { get; set; }
        public List<BasketItem> Items { get; set; }

    }
}
