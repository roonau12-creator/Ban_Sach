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
    public class TacGiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TacGiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.TacGias.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(x => x.TenTacGia.ToLower().Contains(q.Trim().ToLower()));
            }

            ViewBag.TuKhoa = q;
            return View(await query.OrderBy(x => x.TenTacGia).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.TacGias
                .Include(x => x.SachTacGias)
                .ThenInclude(x => x.Sach)
                .FirstOrDefaultAsync(x => x.MaTG == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TacGia model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.TacGias.Add(model);
            await _context.SaveChangesAsync();
            TempData["success"] = "Thêm tác giả thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.TacGias.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TacGia model)
        {
            if (id != model.MaTG) return NotFound();
            if (!ModelState.IsValid) return View(model);
            _context.TacGias.Update(model);
            await _context.SaveChangesAsync();
            TempData["success"] = "Cập nhật tác giả thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.TacGias.FirstOrDefaultAsync(x => x.MaTG == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.TacGias.Include(x => x.SachTacGias).FirstOrDefaultAsync(x => x.MaTG == id);
            if (item == null) return NotFound();
            if (item.SachTacGias.Any())
            {
                TempData["error"] = "Không thể xóa tác giả đang được gán cho sách.";
                return RedirectToAction(nameof(Index));
            }

            _context.TacGias.Remove(item);
            await _context.SaveChangesAsync();
            TempData["success"] = "Xóa tác giả thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
