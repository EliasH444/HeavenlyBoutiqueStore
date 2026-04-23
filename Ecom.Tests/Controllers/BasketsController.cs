using Xunit;
using Moq;
using learning.Controllers;
using learning.Services;
using learning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class BasketsControllerTests
{
    [Fact]
    public async Task AddToBasket_AddsItemCorrectly()
    {
        // Arrange
        var context = InMemoryDbHelper.GetDbContext();

        context.Product.Add(new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 10,
            StockQuantity = 10
        });
        await context.SaveChangesAsync();

        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.GetUserId()).Returns("user1");

        var emailServiceMock = new Mock<IEmailService>();

        var stripeOptions = Options.Create(new StripeSettings
        {
            SecretKey = "test"
        });

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            new Mock<IUserStore<ApplicationUser>>().Object,
            null, null, null, null, null, null, null, null
        );

        var controller = new BasketsController(
            context,
            userServiceMock.Object,
            stripeOptions,
            emailServiceMock.Object,
            userManagerMock.Object
        );

        // Act
        var result = await controller.AddToBasket(1, 2);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("ViewBasket", redirect.ActionName);

        var basket = await context.Basket
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.UserId == "user1");

        Assert.NotNull(basket);
        Assert.Single(basket.Items);
        Assert.Equal(2, basket.Items.First().Quantity);
    }

}