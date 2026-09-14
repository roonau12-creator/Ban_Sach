using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [KiemTraVaiTro(VaiTroNguoiDung.Admin)]
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? trangThai)
        {
            var query = _context.DonHangs.Include(x => x.KhachHang).AsQueryable();
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            ViewBag.TrangThai = trangThai;
            return View(await query.OrderByDescending(x => x.NgayDat).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var don = await LayDon(id.Value);
            if (don == null) return NotFound();
            return View(don);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThai)
        {
            var don = await _context.DonHangs
                .Include(x => x.ChiTietDonHangs)
                .Include(x => x.GiaoHang)
                .FirstOrDefaultAsync(x => x.MaDH == id);
            if (don == null) return NotFound();

            var hopLe = new[]
            {
                TrangThaiDonHang.ChoXacNhan,
                TrangThaiDonHang.DaXacNhan,
                TrangThaiDonHang.DangGiao,
                TrangThaiDonHang.DaGiao,
                TrangThaiDonHang.DaHuy
            };
            if (!hopLe.Contains(trangThai))
            {
                TempData["error"] = "Trạng thái không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (don.TrangThai == TrangThaiDonHang.DaHuy)
            {
                TempData["error"] = "Đơn đã hủy, không thể cập nhật.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (trangThai == TrangThaiDonHang.DaHuy && don.TrangThai != TrangThaiDonHang.DaGiao)
            {
                await CongTon(don.MaDH);
            }

            don.TrangThai = trangThai;
            don.GiaoHang ??= new Models.GiaoHang { MaDH = don.MaDH };

            if (trangThai == TrangThaiDonHang.DangGiao)
            {
                don.GiaoHang.TrangThai = TrangThaiGiaoHang.DangGiao;
            }
            else if (trangThai == TrangThaiDonHang.DaGiao)
            {
                don.GiaoHang.TrangThai = TrangThaiGiaoHang.DaGiao;
                don.GiaoHang.NgayGiao = DateTime.UtcNow;
                don.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
            }
            else if (trangThai == TrangThaiDonHang.DaHuy)
            {
                don.GiaoHang.TrangThai = TrangThaiGiaoHang.ChoGiao;
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Đã cập nhật trạng thái đơn hàng.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatThanhToan(int id, string trangThaiThanhToan)
        {
            var don = await _context.DonHangs.FindAsync(id);
            if (don == null) return NotFound();
            if (don.TrangThai == TrangThaiDonHang.DaHuy)
            {
                TempData["error"] = "Không thể thanh toán đơn đã hủy.";
                return RedirectToAction(nameof(Details), new { id });
            }

            don.TrangThaiThanhToan = trangThaiThanhToan == TrangThaiThanhToan.DaThanhToan
                ? TrangThaiThanhToan.DaThanhToan
                : TrangThaiThanhToan.ChuaThanhToan;
            await _context.SaveChangesAsync();
            TempData["success"] = "Đã cập nhật thanh toán.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatGiaoHang(int id, string trangThai, string? ghiChu)
        {
            var don = await _context.DonHangs.Include(x => x.GiaoHang).FirstOrDefaultAsync(x => x.MaDH == id);
            if (don == null) return NotFound();
            don.GiaoHang ??= new Models.GiaoHang { MaDH = don.MaDH, DonHang = don };
            don.GiaoHang.TrangThai = trangThai;
            don.GiaoHang.GhiChu = ghiChu;
            if (trangThai == TrangThaiGiaoHang.DaGiao)
            {
                don.GiaoHang.NgayGiao = DateTime.UtcNow;
                don.TrangThai = TrangThaiDonHang.DaGiao;
                don.TrangThaiThanhToan = TrangThaiThanhToan.DaThanhToan;
            }
            else if (trangThai == TrangThaiGiaoHang.DangGiao)
            {
                don.TrangThai = TrangThaiDonHang.DangGiao;
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Đã cập nhật giao hàng.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<Models.DonHang?> LayDon(int id)
        {
            return await _context.DonHangs
                .Include(x => x.KhachHang)
                .Include(x => x.GiaoHang)
                .Include(x => x.ChiTietDonHangs).ThenInclude(x => x.Sach)
                .FirstOrDefaultAsync(x => x.MaDH == id);
        }

        private async Task CongTon(int maDH)
        {
            var cts = await _context.ChiTietDonHangs.Where(x => x.MaDH == maDH).ToListAsync();
            foreach (var ct in cts)
            {
                var sach = await _context.Sachs.FindAsync(ct.MaSach);
                if (sach != null)
                {
                    sach.SoLuongTon += ct.SoLuong;
                }
            }
        }
    }
}
