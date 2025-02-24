using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using learning.Data;
using learning.Models;
using learning.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
builder.Services.AddDbContext<learningContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("learningContext")
    ?? throw new InvalidOperationException("Connection string 'learningContext' not found.")));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<learningContext>()
    .AddDefaultTokenProviders();

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserService>();


app.Run();
