namespace learning.Models
{
    public class Purchase
    {
        public int PurchaseID { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<PurchaseItem> Items { get; set; }

    }
}
