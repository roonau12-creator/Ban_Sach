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

        public GioHangController(ApplicationDbContext context, SessionGioHang gioHang)
        {
            _context = context;
            _gioHang = gioHang;
        }

        public IActionResult Index()
        {
            return View(_gioHang.LayGio());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(int maSach, int soLuong = 1)
        {
            if (soLuong < 1) soLuong = 1;
            var sach = await _context.Sachs.FindAsync(maSach);
            if (sach == null) return NotFound();

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
            var sach = await _context.Sachs.FindAsync(maSach);
            if (sach == null) return NotFound();

            var gio = _gioHang.LayGio();
            var item = gio.FirstOrDefault(x => x.MaSach == maSach);
            if (item == null) return RedirectToAction(nameof(Index));

            if (soLuong < 1)
            {
                gio.Remove(item);
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

            _gioHang.LuuGio(gio);
            return RedirectToAction(nameof(Index));
        }
    }
}
