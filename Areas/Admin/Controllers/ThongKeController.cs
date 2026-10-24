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
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime? tuNgay, DateTime? denNgay)
        {
            var from = (tuNgay ?? DateTime.UtcNow.AddDays(-30)).Date;
            var to = (denNgay ?? DateTime.UtcNow.Date).Date.AddDays(1).AddTicks(-1);
            var fromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(to, DateTimeKind.Utc);

            var don = await _context.DonHangs
                .Include(x => x.KhachHang)
                .Where(x => x.NgayDat >= fromUtc && x.NgayDat <= toUtc)
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();

            var model = new ThongKeViewModel
            {
                TuNgay = from,
                DenNgay = to.Date,
                TongDon = don.Count,
                DoanhThu = don.Where(x => x.TrangThai != TrangThaiDonHang.DaHuy &&
                    (x.TrangThaiThanhToan == TrangThaiThanhToan.DaThanhToan || x.TrangThai == TrangThaiDonHang.DaGiao))
                    .Sum(x => x.TongTien),
                DonTheoTrangThai = don.GroupBy(x => x.TrangThai).ToDictionary(g => g.Key, g => g.Count()),
                DonHangs = don
            };

            return View(model);
        }
    }
}
