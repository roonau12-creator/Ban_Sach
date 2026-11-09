using BanSach.Data;
using BanSach.Helpers;
using BanSach.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GioHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionGioHang _gioHang;
        private readonly GioHangService _gioHangService;

        public GioHangController(
            ApplicationDbContext context,
            SessionGioHang gioHang,
            GioHangService gioHangService)
        {
            _context = context;
            _gioHang = gioHang;
            _gioHangService = gioHangService;
        }

        public async Task<IActionResult> Index()
        {
            if (await _gioHangService.DongBoAsync())
            {
                TempData["error"] = "Giỏ hàng đã được cập nhật theo giá và tồn kho hiện tại. Vui lòng kiểm tra lại.";
            }

            return View(_gioHang.LayGio());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(int maSach, int soLuong = 1)
        {
            if (soLuong < 1)
            {
                TempData["error"] = "Số lượng phải lớn hơn 0.";
                return RedirectToAction("Details", "Sach", new { id = maSach });
            }

            var sach = await _context.Sachs
                .Include(x => x.NhomSach)
                .FirstOrDefaultAsync(x => x.MaSach == maSach);
            if (sach == null) return NotFound();
            if (!GioHangService.CoTheBan(new GioHangItem { SoLuong = 1 }, sach))
            {
                TempData["error"] = "Sách hiện không được kinh doanh hoặc đã hết hàng.";
                return RedirectToAction("Details", "Sach", new { id = maSach });
            }

            var gio = _gioHang.LayGio();
            var item = gio.FirstOrDefault(x => x.MaSach == maSach);
            var tong = (item?.SoLuong ?? 0) + soLuong;
            if (tong > sach.SoLuongTon)
            {
                TempData["error"] = $"Chỉ còn {sach.SoLuongTon} cuốn trong kho.";
                return RedirectToAction("Details", "Sach", new { id = maSach });
            }

            if (item == null)
            {
                gio.Add(new GioHangItem
                {
                    MaSach = sach.MaSach,
                    TenSach = sach.TenSach,
                    AnhBia = sach.AnhBia,
                    GiaBan = sach.GiaBan,
                    SoLuong = soLuong
                });
            }
            else
            {
                item.SoLuong = tong;
                item.TenSach = sach.TenSach;
                item.AnhBia = sach.AnhBia;
                item.GiaBan = sach.GiaBan;
            }

            _gioHang.LuuGio(gio);
            TempData["success"] = "Đã thêm sách vào giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Xoa(int maSach)
        {
            var gio = _gioHang.LayGio();
            gio.RemoveAll(x => x.MaSach == maSach);
            _gioHang.LuuGio(gio);
            TempData["success"] = "Đã xóa sách khỏi giỏ.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhat(int maSach, int soLuong)
        {
            if (soLuong < 1)
            {
                TempData["error"] = "Số lượng phải lớn hơn 0. Hãy dùng nút xóa để bỏ sách khỏi giỏ.";
                return RedirectToAction(nameof(Index));
            }

            var sach = await _context.Sachs
                .Include(x => x.NhomSach)
                .FirstOrDefaultAsync(x => x.MaSach == maSach);
            if (sach == null)
            {
                await _gioHangService.DongBoAsync();
                TempData["error"] = "Sách không còn tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            var gio = _gioHang.LayGio();
            var item = gio.FirstOrDefault(x => x.MaSach == maSach);
            if (item == null) return RedirectToAction(nameof(Index));

            if (!GioHangService.CoTheBan(item, sach))
            {
                gio.Remove(item);
                TempData["error"] = "Sách hiện không được kinh doanh hoặc đã hết hàng.";
            }
            else if (soLuong > sach.SoLuongTon)
            {
                TempData["error"] = $"Chỉ còn {sach.SoLuongTon} cuốn trong kho.";
                item.SoLuong = sach.SoLuongTon;
            }
            else
            {
                item.SoLuong = soLuong;
            }

            item.TenSach = sach.TenSach;
            item.AnhBia = sach.AnhBia;
            item.GiaBan = sach.GiaBan;
            _gioHang.LuuGio(gio);
            return RedirectToAction(nameof(Index));
        }
    }
}
