using Microsoft.EntityFrameworkCore;

namespace learning.Models
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string Url { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}