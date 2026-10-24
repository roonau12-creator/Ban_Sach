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
    public class ChuDeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChuDeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.ChuDes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(x => x.TenChuDe.ToLower().Contains(q.Trim().ToLower()));
            }

            ViewBag.TuKhoa = q;
            var danhSach = await query.OrderByDescending(x => x.MaCD).ToListAsync();
            return View(danhSach);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes
                .Include(x => x.Sachs)
                .FirstOrDefaultAsync(x => x.MaCD == id);

            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChuDe chuDe)
        {
            if (ModelState.IsValid)
            {
                _context.ChuDes.Add(chuDe);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm chủ đề thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(chuDe);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes.FindAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChuDe chuDe)
        {
            if (id != chuDe.MaCD)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.ChuDes.Update(chuDe);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật chủ đề thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(chuDe);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes.FirstOrDefaultAsync(x => x.MaCD == id);
            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chuDe = await _context.ChuDes.Include(x => x.Sachs).FirstOrDefaultAsync(x => x.MaCD == id);
            if (chuDe == null)
            {
                return NotFound();
            }

            if (chuDe.Sachs.Any())
            {
                TempData["error"] = "Không thể xóa chủ đề đang có sách.";
                return RedirectToAction(nameof(Index));
            }

            _context.ChuDes.Remove(chuDe);
            await _context.SaveChangesAsync();
            TempData["success"] = "Xóa chủ đề thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
