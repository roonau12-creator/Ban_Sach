using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    [KiemTraDangNhap]
    public class SoDienThoaiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SoDienThoaiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null) return RedirectToAction("HoSo", "TaiKhoan");
            var list = await _context.SoDienThoais.Where(x => x.MaKH == maKH).OrderByDescending(x => x.MacDinh).ToListAsync();
            return View(list);
        }

        public IActionResult Create() => View(new SoDienThoai());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SoDienThoai model)
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null) return RedirectToAction("HoSo", "TaiKhoan");
            if (!ModelState.IsValid) return View(model);

            model.MaKH = maKH.Value;
            if (model.MacDinh)
            {
                await BoMacDinh(maKH.Value);
            }

            _context.SoDienThoais.Add(model);
            await _context.SaveChangesAsync();
            TempData["success"] = "Thêm số điện thoại thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var item = await LayCuaToi(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SoDienThoai model)
        {
            var item = await LayCuaToi(id);
            if (item == null) return NotFound();
            if (!ModelState.IsValid) return View(model);

            item.SoDT = model.SoDT;
            item.MacDinh = model.MacDinh;
            if (model.MacDinh)
            {
                await BoMacDinh(item.MaKH, item.MaSDT);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Cập nhật số điện thoại thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await LayCuaToi(id);
            if (item == null) return NotFound();
            _context.SoDienThoais.Remove(item);
            await _context.SaveChangesAsync();
            TempData["success"] = "Đã xóa số điện thoại.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<SoDienThoai?> LayCuaToi(int? id)
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (id == null || maKH == null) return null;
            return await _context.SoDienThoais.FirstOrDefaultAsync(x => x.MaSDT == id && x.MaKH == maKH);
        }

        private async Task BoMacDinh(int maKH, int? except = null)
        {
            var list = await _context.SoDienThoais.Where(x => x.MaKH == maKH && x.MacDinh && x.MaSDT != except).ToListAsync();
            foreach (var sdt in list) sdt.MacDinh = false;
        }
    }
}
