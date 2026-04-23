using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using learning.Services;

public class UserServiceTests
{
    [Fact]
    public void GetUserId_ReturnsId()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123")
        }));

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var service = new UserService(accessorMock.Object);

        var result = service.GetUserId();

        Assert.Equal("user123", result);
    }
}