using Microsoft.EntityFrameworkCore;
using learning.Data;

public static class InMemoryDbHelper
{
    public static learningContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<learningContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new learningContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}