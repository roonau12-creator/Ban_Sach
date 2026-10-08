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
    public class DiaChiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiaChiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null) return RedirectToAction("HoSo", "TaiKhoan");
            var list = await _context.DiaChis.Where(x => x.MaKH == maKH).OrderByDescending(x => x.MacDinh).ToListAsync();
            return View(list);
        }

        public IActionResult Create() => View(new DiaChi());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiaChi model)
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null) return RedirectToAction("HoSo", "TaiKhoan");
            if (!ModelState.IsValid) return View(model);

            model.MaKH = maKH.Value;
            if (model.MacDinh)
            {
                await BoMacDinh(maKH.Value);
            }

            _context.DiaChis.Add(model);
            await _context.SaveChangesAsync();
            TempData["success"] = "Thêm địa chỉ thành công!";
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
        public async Task<IActionResult> Edit(int id, DiaChi model)
        {
            var item = await LayCuaToi(id);
            if (item == null) return NotFound();
            if (!ModelState.IsValid) return View(model);

            item.DiaChiChiTiet = model.DiaChiChiTiet;
            item.MacDinh = model.MacDinh;
            if (model.MacDinh)
            {
                await BoMacDinh(item.MaKH, item.MaDC);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Cập nhật địa chỉ thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await LayCuaToi(id);
            if (item == null) return NotFound();
            _context.DiaChis.Remove(item);
            await _context.SaveChangesAsync();
            TempData["success"] = "Đã xóa địa chỉ.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<DiaChi?> LayCuaToi(int? id)
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (id == null || maKH == null) return null;
            return await _context.DiaChis.FirstOrDefaultAsync(x => x.MaDC == id && x.MaKH == maKH);
        }

        private async Task BoMacDinh(int maKH, int? except = null)
        {
            var list = await _context.DiaChis.Where(x => x.MaKH == maKH && x.MacDinh && x.MaDC != except).ToListAsync();
            foreach (var dc in list) dc.MacDinh = false;
        }
    }
}
