namespace learning.Models
{
    public class PurchaseItem
    {
        public int PurchaseItemID { get; set; }
        public int PurchaseID { get; set; }
        public Purchase Purchase { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }


    }
}
