using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using learning.Models;

namespace learning.Data
{
    public class learningContext : IdentityDbContext<ApplicationUser>  // ✅ Correct inheritance
    {
        public learningContext(DbContextOptions<learningContext> options)
            : base(options)
        {
        }
        //add to
        // Ensure this model exists in your project, otherwise comment it out or remove it
        public DbSet<Product> Product { get; set; } = default!;
        public DbSet<Basket> Basket { get; set; } = default!;
        public DbSet<BasketItem> BasketItem { get; set; } = default!;

    }
}
