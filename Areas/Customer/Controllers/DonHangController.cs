using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models;
using BanSach.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    [KiemTraDangNhap]
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionGioHang _gioHang;

        public DonHangController(ApplicationDbContext context, SessionGioHang gioHang)
        {
            _context = context;
            _gioHang = gioHang;
        }

        public async Task<IActionResult> DatHang()
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null)
            {
                TempData["error"] = "Tài khoản quản trị không đặt hàng từ giỏ khách. Hãy dùng tài khoản khách hàng.";
                return RedirectToAction("Index", "GioHang");
            }

            var items = _gioHang.LayGio();
            if (!items.Any())
            {
                TempData["error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "GioHang");
            }

            var diaChis = await _context.DiaChis.Where(x => x.MaKH == maKH).ToListAsync();
            var sdts = await _context.SoDienThoais.Where(x => x.MaKH == maKH).ToListAsync();
            if (!diaChis.Any() || !sdts.Any())
            {
                TempData["error"] = "Hãy thêm ít nhất một địa chỉ và một số điện thoại trước khi đặt hàng.";
                return RedirectToAction("Index", !diaChis.Any() ? "DiaChi" : "SoDienThoai");
            }

            var model = new DatHangViewModel
            {
                Items = items,
                TongTien = items.Sum(x => x.ThanhTien),
                MaDC = diaChis.FirstOrDefault(x => x.MacDinh)?.MaDC ?? diaChis[0].MaDC,
                MaSDT = sdts.FirstOrDefault(x => x.MacDinh)?.MaSDT ?? sdts[0].MaSDT,
                DiaChis = new SelectList(diaChis, "MaDC", "DiaChiChiTiet"),
                SoDienThoais = new SelectList(sdts, "MaSDT", "SoDT")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatHang(DatHangViewModel model)
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null) return RedirectToAction("DangNhap", "TaiKhoan");

            var items = _gioHang.LayGio();
            if (!items.Any())
            {
                TempData["error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "GioHang");
            }

            var diaChi = await _context.DiaChis.FirstOrDefaultAsync(x => x.MaDC == model.MaDC && x.MaKH == maKH);
            var sdt = await _context.SoDienThoais.FirstOrDefaultAsync(x => x.MaSDT == model.MaSDT && x.MaKH == maKH);
            if (diaChi == null || sdt == null)
            {
                TempData["error"] = "Địa chỉ hoặc số điện thoại không hợp lệ.";
                return RedirectToAction(nameof(DatHang));
            }

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in items)
                {
                    var sach = await _context.Sachs.FirstOrDefaultAsync(x => x.MaSach == item.MaSach);
                    if (sach == null || sach.SoLuongTon < item.SoLuong)
                    {
                        TempData["error"] = $"Sách \"{item.TenSach}\" không đủ số lượng tồn.";
                        return RedirectToAction("Index", "GioHang");
                    }

                    sach.SoLuongTon -= item.SoLuong;
                }

                var don = new DonHang
                {
                    MaKH = maKH.Value,
                    NgayDat = DateTime.UtcNow,
                    TrangThai = TrangThaiDonHang.ChoXacNhan,
                    TongTien = items.Sum(x => x.ThanhTien),
                    PhuongThucThanhToan = model.PhuongThucThanhToan == PhuongThucThanhToan.ChuyenKhoan
                        ? PhuongThucThanhToan.ChuyenKhoan
                        : PhuongThucThanhToan.COD,
                    TrangThaiThanhToan = model.PhuongThucThanhToan == PhuongThucThanhToan.ChuyenKhoan
                        ? TrangThaiThanhToan.DaThanhToan
                        : TrangThaiThanhToan.ChuaThanhToan,
                    MaDC = diaChi.MaDC,
                    MaSDT = sdt.MaSDT,
                    DiaChiGiao = diaChi.DiaChiChiTiet,
                    SoDienThoaiGiao = sdt.SoDT,
                    GhiChu = model.GhiChu,
                    ChiTietDonHangs = items.Select(x => new ChiTietDonHang
                    {
                        MaSach = x.MaSach,
                        SoLuong = x.SoLuong,
                        DonGia = x.GiaBan
                    }).ToList(),
                    GiaoHang = new GiaoHang
                    {
                        TrangThai = TrangThaiGiaoHang.ChoGiao
                    }
                };

                _context.DonHangs.Add(don);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                _gioHang.XoaGio();
                TempData["success"] = "Đặt hàng thành công!";
                return RedirectToAction(nameof(ChiTiet), new { id = don.MaDH });
            }
            catch
            {
                await tx.RollbackAsync();
                TempData["error"] = "Không thể đặt hàng. Vui lòng thử lại.";
                return RedirectToAction("Index", "GioHang");
            }
        }

        public async Task<IActionResult> LichSu()
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null) return RedirectToAction("HoSo", "TaiKhoan");
            var list = await _context.DonHangs
                .Where(x => x.MaKH == maKH)
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> ChiTiet(int? id)
        {
            var don = await LayDonCuaToi(id);
            if (don == null) return NotFound();
            return View(don);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            var don = await LayDonCuaToi(id);
            if (don == null) return NotFound();
            if (don.TrangThai != TrangThaiDonHang.ChoXacNhan)
            {
                TempData["error"] = "Chỉ hủy được đơn đang chờ xác nhận.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            don.TrangThai = TrangThaiDonHang.DaHuy;
            foreach (var ct in don.ChiTietDonHangs)
            {
                if (ct.Sach != null)
                {
                    ct.Sach.SoLuongTon += ct.SoLuong;
                }
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Đã hủy đơn hàng.";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        private async Task<DonHang?> LayDonCuaToi(int? id)
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (id == null || maKH == null) return null;
            return await _context.DonHangs
                .Include(x => x.GiaoHang)
                .Include(x => x.ChiTietDonHangs).ThenInclude(x => x.Sach)
                .FirstOrDefaultAsync(x => x.MaDH == id && x.MaKH == maKH);
        }
    }
}
