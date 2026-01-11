using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using learning.Models;

namespace learning.Data
{
    public static class ProductSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<learningContext>();

            // Apply pending migrations (optional)
            await db.Database.MigrateAsync();

            // Guard to avoid duplicate seeding
            if (await db.Product.AnyAsync(p => p.Name == "Khamis Special"))
                return;

            var khamisProduct = new Product
            {
               
            };

            db.Product.Add(khamisProduct);
            await db.SaveChangesAsync();
            Console.WriteLine("Seeded Khamis Special product.");
        }
    }
}