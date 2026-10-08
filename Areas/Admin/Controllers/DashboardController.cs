using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [KiemTraVaiTro(VaiTroNguoiDung.Admin)]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var donHopLe = _context.DonHangs.Where(x => x.TrangThai != TrangThaiDonHang.DaHuy);
            var model = new DashboardViewModel
            {
                TongSach = await _context.Sachs.CountAsync(),
                TongKhachHang = await _context.KhachHangs.CountAsync(),
                TongDonHang = await _context.DonHangs.CountAsync(),
                DoanhThu = await donHopLe
                    .Where(x => x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan || x.TrangThai == TrangThaiDonHang.DaGiao)
                    .SumAsync(x => (decimal?)x.TongTien) ?? 0,
                DonHangMois = await _context.DonHangs
                    .Include(x => x.KhachHang)
                    .OrderByDescending(x => x.NgayDat)
                    .Take(8)
                    .ToListAsync(),
                SachBanChays = await _context.ChiTietDonHangs
                    .Where(x => x.DonHang != null && x.DonHang.TrangThai != TrangThaiDonHang.DaHuy)
                    .GroupBy(x => new { x.MaSach, x.Sach!.TenSach })
                    .Select(g => new SachBanChayItem
                    {
                        MaSach = g.Key.MaSach,
                        TenSach = g.Key.TenSach,
                        SoLuongBan = g.Sum(x => x.SoLuong),
                        DoanhThu = g.Sum(x => x.DonGia * x.SoLuong)
                    })
                    .OrderByDescending(x => x.SoLuongBan)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}
