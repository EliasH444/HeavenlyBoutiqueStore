namespace learning.Models
{
    public class BasketItem
    {
        
        public int BasketItemId { get; set; }
        //Foreign key. Resides in Product.
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int BasketId { get; set; }
        public Basket Basket { get; set; }

        public int  Quantity { get; set; }
    }
}
