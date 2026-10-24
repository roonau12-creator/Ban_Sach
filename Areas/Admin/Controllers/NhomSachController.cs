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
    public class NhomSachController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhomSachController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.NhomSachs.OrderBy(x => x.ThuTu).ThenBy(x => x.TenNhom).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.NhomSachs.Include(x => x.Sachs).FirstOrDefaultAsync(x => x.MaNhom == id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhomSach model)
        {
            if (!ModelState.IsValid) return View(model);

            var max = await _context.NhomSachs.MaxAsync(x => (int?)x.ThuTu) ?? 0;
            model.ThuTu = max + 1;
            _context.NhomSachs.Add(model);
            await _context.SaveChangesAsync();
            TempData["success"] = "Thêm nhóm sách thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.NhomSachs.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhomSach model)
        {
            if (id != model.MaNhom) return NotFound();
            if (!ModelState.IsValid) return View(model);

            _context.NhomSachs.Update(model);
            await _context.SaveChangesAsync();
            TempData["success"] = "Cập nhật nhóm sách thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.NhomSachs.FirstOrDefaultAsync(x => x.MaNhom == id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.NhomSachs.Include(x => x.Sachs).FirstOrDefaultAsync(x => x.MaNhom == id);
            if (item == null) return NotFound();
            if (item.Sachs.Any())
            {
                TempData["error"] = "Không thể xóa nhóm đang có sách.";
                return RedirectToAction(nameof(Index));
            }

            _context.NhomSachs.Remove(item);
            await _context.SaveChangesAsync();
            TempData["success"] = "Xóa nhóm sách thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatTat(int id)
        {
            var item = await _context.NhomSachs.FindAsync(id);
            if (item == null) return NotFound();
            item.TrangThai = !item.TrangThai;
            await _context.SaveChangesAsync();
            TempData["success"] = item.TrangThai ? "Đã bật nhóm sách." : "Đã tắt nhóm sách.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SapXep(int id, string huong)
        {
            var list = await _context.NhomSachs.OrderBy(x => x.ThuTu).ThenBy(x => x.MaNhom).ToListAsync();
            var index = list.FindIndex(x => x.MaNhom == id);
            if (index < 0) return NotFound();

            var swap = huong == "up" ? index - 1 : index + 1;
            if (swap >= 0 && swap < list.Count)
            {
                (list[index].ThuTu, list[swap].ThuTu) = (list[swap].ThuTu, list[index].ThuTu);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
