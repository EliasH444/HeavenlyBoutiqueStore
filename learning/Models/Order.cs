namespace learning.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;         
        public int OrderStatus { get; set; }
        public Order() { }
    }
}
