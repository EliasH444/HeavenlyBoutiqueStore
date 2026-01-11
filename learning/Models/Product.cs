using Microsoft.EntityFrameworkCore;

namespace learning.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

    }
}
