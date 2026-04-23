using Xunit;
using learning.Services;
using learning.Models;
using System.Threading.Tasks;
using System.Linq;

public class SearchServiceTests
{
    [Fact]
    public async Task SearchProductsAsync_ReturnsMatchingProducts()
    {
        // Arrange
        var context = InMemoryDbHelper.GetDbContext();

        context.Product.AddRange(
            new Product { ProductId = 1, Name = "Laptop", Description = "Gaming laptop" },
            new Product { ProductId = 2, Name = "Phone", Description = "Smartphone device" }
        );
        await context.SaveChangesAsync();

        var service = new SearchService(context);

        // Act
        var result = await service.SearchProductsAsync("Laptop");

        // Assert
        Assert.Single(result);
        Assert.Equal("Laptop", result.First().Name);
    }

    [Fact]
    public async Task SearchProductsAsync_EmptyQuery_ReturnsEmpty()
    {
        var context = InMemoryDbHelper.GetDbContext();
        var service = new SearchService(context);

        var result = await service.SearchProductsAsync("");

        Assert.Empty(result);
    }
}