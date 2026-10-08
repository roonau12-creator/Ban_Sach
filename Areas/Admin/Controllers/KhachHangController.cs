using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [KiemTraVaiTro(VaiTroNguoiDung.Admin)]
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhachHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.KhachHangs.Include(x => x.TaiKhoan).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                var key = q.Trim().ToLower();
                query = query.Where(x =>
                    x.HoTen.ToLower().Contains(key) ||
                    x.TaiKhoan!.Email.ToLower().Contains(key) ||
                    x.TaiKhoan.TenDangNhap.ToLower().Contains(key));
            }

            ViewBag.TuKhoa = q;
            return View(await query.OrderByDescending(x => x.MaKH).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var kh = await _context.KhachHangs
                .Include(x => x.TaiKhoan)
                .Include(x => x.DiaChis)
                .Include(x => x.SoDienThoais)
                .Include(x => x.DonHangs)
                .FirstOrDefaultAsync(x => x.MaKH == id);
            if (kh == null) return NotFound();
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatTat(int id)
        {
            var kh = await _context.KhachHangs.Include(x => x.TaiKhoan).FirstOrDefaultAsync(x => x.MaKH == id);
            if (kh?.TaiKhoan == null) return NotFound();
            if (kh.TaiKhoan.VaiTro == VaiTroNguoiDung.Admin)
            {
                TempData["error"] = "Không thể khóa tài khoản quản trị.";
                return RedirectToAction(nameof(Index));
            }

            kh.TaiKhoan.TrangThai = !kh.TaiKhoan.TrangThai;
            await _context.SaveChangesAsync();
            TempData["success"] = kh.TaiKhoan.TrangThai ? "Đã mở khóa tài khoản." : "Đã khóa tài khoản.";
            return RedirectToAction(nameof(Index));
        }
    }
}
