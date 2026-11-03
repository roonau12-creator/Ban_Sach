using System.Security.Claims;
using BanSach.Data;
using BanSach.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Customer.Controllers;

[Area("Customer")]
public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

    public AccountController(
        ApplicationDbContext context,
        IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string normalizedEmail = model.Email.Trim().ToUpperInvariant();
        ApplicationUser? user = await _context.Users
            .SingleOrDefaultAsync(account => account.NormalizedEmail == normalizedEmail);
        if (user == null ||
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password)
                == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
            return View(model);
        }

        await SignInAsync(user);
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home", new { area = "Customer" });
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string email = model.Email.Trim();
        string normalizedEmail = email.ToUpperInvariant();
        if (await _context.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail))
        {
            ModelState.AddModelError(nameof(model.Email), "Email này đã được đăng ký.");
            return View(model);
        }

        ApplicationUser user = new()
        {
            Email = email,
            NormalizedEmail = normalizedEmail,
            Role = UserRoles.Customer,
            CreatedAtUtc = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await SignInAsync(user);
        return RedirectToAction("Index", "Home", new { area = "Customer" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task SignInAsync(ApplicationUser user)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        ];
        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}
