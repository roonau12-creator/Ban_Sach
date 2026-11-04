using BanSach.Data;
using BanSach.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthorizationPolicies.ManageBooks)]
public class SachController : Controller
{
    private readonly ApplicationDbContext _context;

    public SachController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<Sach> books = await _context.Sachs
            .Include(book => book.ChuDe)
            .OrderBy(book => book.TenSach)
            .ToListAsync();

        return View(books);
    }

    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();
        return View(new Sach());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Sach book)
    {
        if (!await _context.ChuDes.AnyAsync(category => category.MaCD == book.MaCD))
        {
            ModelState.AddModelError(nameof(book.MaCD), "Danh mục được chọn không tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(book);
        }

        NormalizeBook(book);
        book.MaSach = 0;
        book.ChuDe = null;
        _context.Sachs.Add(book);
        await _context.SaveChangesAsync();

        TempData["success"] = "Thêm sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Sach? book = await _context.Sachs.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        await LoadCategoriesAsync();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Sach book)
    {
        if (id != book.MaSach)
        {
            return NotFound();
        }

        if (!await _context.ChuDes.AnyAsync(category => category.MaCD == book.MaCD))
        {
            ModelState.AddModelError(nameof(book.MaCD), "Danh mục được chọn không tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(book);
        }

        Sach? existingBook = await _context.Sachs.FindAsync(id);
        if (existingBook == null)
        {
            return NotFound();
        }

        NormalizeBook(book);
        existingBook.TenSach = book.TenSach;
        existingBook.MoTa = book.MoTa;
        existingBook.MaCD = book.MaCD;

        await _context.SaveChangesAsync();
        TempData["success"] = "Cập nhật sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCategoriesAsync()
    {
        ViewBag.Categories = new SelectList(
            await _context.ChuDes.OrderBy(category => category.TenChuDe).ToListAsync(),
            nameof(ChuDe.MaCD),
            nameof(ChuDe.TenChuDe));
    }

    private static void NormalizeBook(Sach book)
    {
        book.TenSach = book.TenSach.Trim();
        book.MoTa = string.IsNullOrWhiteSpace(book.MoTa) ? null : book.MoTa.Trim();
    }
}
