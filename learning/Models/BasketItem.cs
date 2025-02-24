namespace learning.Models
{
    public class BasketItem
    {
        //Foreign key. Resides in Product.
        public int BasketItemId { get; set; }
        public int ProductId { get; set; }
        public Product product { get; set; }
        public int BasketId { get; set; }
        public Basket basket { get; set; }
    }
}
