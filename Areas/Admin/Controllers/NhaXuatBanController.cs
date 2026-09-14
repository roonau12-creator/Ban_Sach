using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [KiemTraVaiTro(VaiTroNguoiDung.Admin)]
    public class NhaXuatBanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhaXuatBanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.NhaXuatBans.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(x => x.TenNXB.ToLower().Contains(q.Trim().ToLower()));
            }

            ViewBag.TuKhoa = q;
            return View(await query.OrderBy(x => x.TenNXB).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.NhaXuatBans.Include(x => x.Sachs).FirstOrDefaultAsync(x => x.MaNXB == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaXuatBan model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.NhaXuatBans.Add(model);
            await _context.SaveChangesAsync();
            TempData["success"] = "Thêm nhà xuất bản thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.NhaXuatBans.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhaXuatBan model)
        {
            if (id != model.MaNXB) return NotFound();
            if (!ModelState.IsValid) return View(model);
            _context.NhaXuatBans.Update(model);
            await _context.SaveChangesAsync();
            TempData["success"] = "Cập nhật nhà xuất bản thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.NhaXuatBans.FirstOrDefaultAsync(x => x.MaNXB == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.NhaXuatBans.Include(x => x.Sachs).FirstOrDefaultAsync(x => x.MaNXB == id);
            if (item == null) return NotFound();
            if (item.Sachs.Any())
            {
                TempData["error"] = "Không thể xóa nhà xuất bản đang có sách.";
                return RedirectToAction(nameof(Index));
            }

            _context.NhaXuatBans.Remove(item);
            await _context.SaveChangesAsync();
            TempData["success"] = "Xóa nhà xuất bản thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
