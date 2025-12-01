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
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();


var app = builder.Build();

// ======= ADMIN SEEDING CODE STARTS =======
async Task SeedAdminUserAsync()
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    const string adminRoleName = "Admin";

    // Create role if not exist
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
// ======= ADMIN SEEDING CODE ENDS =======


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

// Run the seeding
await SeedAdminUserAsync();

app.Run();
