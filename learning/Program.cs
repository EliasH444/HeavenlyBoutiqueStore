using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using learning.Data;
using learning.Models;
using learning.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
builder.Services.AddDbContext<learningContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("learningContext")
        ?? throw new InvalidOperationException("Connection string 'learningContext' not found.")));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<learningContext>()
    .AddDefaultTokenProviders();

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISearch, SearchService>();
builder.Services.AddScoped<IImageService, LocalImageService>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

var app = builder.Build();

// ===== 1️⃣ Apply migrations first =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<learningContext>();
    db.Database.Migrate(); // Creates all tables if they don't exist
}

// ===== 2️⃣ Seed admin user after migrations =====
async Task SeedAdminUserAsync()
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    const string adminRoleName = "Admin";

    // Create role if not exists
    if (!await roleManager.RoleExistsAsync(adminRoleName))
        await roleManager.CreateAsync(new IdentityRole(adminRoleName));

    // Create admin user
    var adminEmail = "shadiaH444@outlook.com";
    var adminPassword = "Admin123!";
    var FullName = "Shadia Hassan";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = FullName
        };

        await userManager.CreateAsync(adminUser, adminPassword);
    }

    // Assign role if user does not have it
    if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
        await userManager.AddToRoleAsync(adminUser, adminRoleName);
}

// ===== 3️⃣ Configure middleware =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ===== 4️⃣ Run seeding =====
await SeedAdminUserAsync();

app.Run();
