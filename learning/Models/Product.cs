using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

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
        public Category? Category { get; set; }

        // Replaces ImageUrl
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

        [NotMapped]
        public ProductImage? PrimaryImage =>
            Images.FirstOrDefault(i => i.IsPrimary) ?? Images.FirstOrDefault();

        [NotMapped]
        public string PrimaryImageUrl => PrimaryImage?.Url ?? "/images/placeholder.jpg";
        //public List<string> images { get; set; } = new List<string>();

    }
}
