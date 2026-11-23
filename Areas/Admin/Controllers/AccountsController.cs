using System.Security.Claims;
using BanSach.Data;
using BanSach.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthorizationPolicies.ManageStaff)]
public class AccountsController : Controller
{
    private static readonly string[] AssignableRoles =
    [
        UserRoles.Administrator,
        UserRoles.Employee,
        UserRoles.Customer
    ];

    private readonly ApplicationDbContext _context;

    public AccountsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<AccountListItem> accounts = await _context.Users
            .OrderBy(user => user.Email)
            .Select(user => new AccountListItem
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                CreatedAtUtc = user.CreatedAtUtc
            })
            .ToListAsync();

        return View(accounts);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(ChangeUserRoleViewModel model)
    {
        string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!ModelState.IsValid || !AssignableRoles.Contains(model.Role, StringComparer.Ordinal))
        {
            TempData["error"] = "Vai trò không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        if (currentUserId == model.UserId.ToString() &&
            model.Role != UserRoles.Administrator)
        {
            TempData["error"] = "Bạn không thể tự hạ quyền quản trị của chính mình.";
            return RedirectToAction(nameof(Index));
        }

        ApplicationUser? account = await _context.Users.FindAsync(model.UserId);
        if (account == null)
        {
            return NotFound();
        }

        if (account.Role == UserRoles.Administrator &&
            model.Role != UserRoles.Administrator &&
            await _context.Users.CountAsync(user => user.Role == UserRoles.Administrator) <= 1)
        {
            TempData["error"] = "Không thể hạ quyền quản trị viên cuối cùng.";
            return RedirectToAction(nameof(Index));
        }

        account.Role = model.Role;
        await _context.SaveChangesAsync();
        TempData["success"] = $"Đã cập nhật vai trò cho {account.Email}.";
        return RedirectToAction(nameof(Index));
    }
}
