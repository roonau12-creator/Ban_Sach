using BanSach.Data;
using BanSach.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(
    options=>options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
builder.Services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Events.OnValidatePrincipal = async context =>
        {
            ClaimsIdentity? identity = context.Principal?.Identity as ClaimsIdentity;
            string? userId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out int parsedUserId))
            {
                context.RejectPrincipal();
                return;
            }

            ApplicationDbContext db = context.HttpContext.RequestServices
                .GetRequiredService<ApplicationDbContext>();
            ApplicationUser? user = await db.Users.FindAsync(parsedUserId);
            if (user == null)
            {
                context.RejectPrincipal();
                return;
            }

            Claim? roleClaim = identity!.FindFirst(ClaimTypes.Role);
            if (roleClaim?.Value != user.Role)
            {
                if (roleClaim != null)
                {
                    identity.RemoveClaim(roleClaim);
                }

                identity.AddClaim(new Claim(ClaimTypes.Role, user.Role));
                context.ShouldRenew = true;
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.ManageBooks, policy =>
        policy.RequireRole(UserRoles.Administrator, UserRoles.Employee));
    options.AddPolicy(AuthorizationPolicies.ManageStaff, policy =>
        policy.RequireRole(UserRoles.Administrator));
    options.AddPolicy(AuthorizationPolicies.ManageSystemSettings, policy =>
        policy.RequireRole(UserRoles.Administrator));
    options.AddPolicy(AuthorizationPolicies.ViewRevenueReports, policy =>
        policy.RequireRole(UserRoles.Administrator));
});
var app = builder.Build();

string? bootstrapEmail = builder.Configuration["BootstrapAdmin:Email"];
string? bootstrapPassword = builder.Configuration["BootstrapAdmin:Password"];
if (string.IsNullOrWhiteSpace(bootstrapEmail) != string.IsNullOrWhiteSpace(bootstrapPassword))
{
    throw new InvalidOperationException(
        "Both BootstrapAdmin:Email and BootstrapAdmin:Password must be configured together.");
}

if (!string.IsNullOrWhiteSpace(bootstrapEmail) && !string.IsNullOrWhiteSpace(bootstrapPassword))
{
    if (bootstrapPassword.Length is < 12 or > 128)
    {
        throw new InvalidOperationException(
            "The bootstrap administrator password must be between 12 and 128 characters.");
    }

    using IServiceScope scope = app.Services.CreateScope();
    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    IPasswordHasher<ApplicationUser> passwordHasher =
        scope.ServiceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>();
    string normalizedEmail = bootstrapEmail.Trim().ToUpperInvariant();
    ApplicationUser? administrator = await db.Users
        .SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail);

    if (administrator == null)
    {
        administrator = new ApplicationUser
        {
            Email = bootstrapEmail.Trim(),
            NormalizedEmail = normalizedEmail,
            Role = UserRoles.Administrator,
            CreatedAtUtc = DateTime.UtcNow
        };
        administrator.PasswordHash = passwordHasher.HashPassword(administrator, bootstrapPassword);
        db.Users.Add(administrator);
    }
    else
    {
        administrator.Role = UserRoles.Administrator;
    }

    await db.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults:new {area="Customer"})
    .WithStaticAssets();


app.Run();
