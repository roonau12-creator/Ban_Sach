using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [KiemTraVaiTro(VaiTroNguoiDung.Admin)]
    public class DanhGiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DanhGiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(bool? an)
        {
            var query = _context.DanhGias
                .Include(x => x.Sach)
                .Include(x => x.TaiKhoan).ThenInclude(x => x!.KhachHang)
                .AsQueryable();

            if (an.HasValue)
            {
                query = query.Where(x => x.An == an.Value);
            }

            return View(await query.OrderByDescending(x => x.NgayDanhGia).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThaiAn(int id)
        {
            var danhGia = await _context.DanhGias.FindAsync(id);
            if (danhGia == null) return NotFound();

            danhGia.An = !danhGia.An;
            await _context.SaveChangesAsync();
            TempData["success"] = danhGia.An
                ? "Đã ẩn nhận xét."
                : "Đã hiện nhận xét.";
            return RedirectToAction(nameof(Index));
        }
    }
}
