using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [KiemTraVaiTro(VaiTroNguoiDung.Admin)]
    public class KhuyenMaiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhuyenMaiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var khuyenMais = await _context.KhuyenMais
                .AsNoTracking()
                .OrderByDescending(x => x.MaKM)
                .ToListAsync();
            return View(khuyenMais);
        }

        public IActionResult Create()
        {
            return View(new KhuyenMai());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("MaCode,TenKhuyenMai,LoaiGiamGia,GiaTriGiam,GiamToiDa,DonHangToiThieu,NgayBatDau,NgayKetThuc,DangHoatDong")]
            KhuyenMai khuyenMai)
        {
            ChuanHoa(khuyenMai);
            if (await _context.KhuyenMais.AnyAsync(x => x.MaCode == khuyenMai.MaCode))
            {
                ModelState.AddModelError(nameof(khuyenMai.MaCode), "Mã khuyến mãi này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                _context.KhuyenMais.Add(khuyenMai);
                await _context.SaveChangesAsync();
                TempData["success"] = "Đã tạo chương trình khuyến mãi.";
                return RedirectToAction(nameof(Index));
            }

            return View(khuyenMai);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var khuyenMai = await _context.KhuyenMais.FindAsync(id);
            return khuyenMai == null ? NotFound() : View(khuyenMai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("MaCode,TenKhuyenMai,LoaiGiamGia,GiaTriGiam,GiamToiDa,DonHangToiThieu,NgayBatDau,NgayKetThuc,DangHoatDong")]
            KhuyenMai input)
        {
            var khuyenMai = await _context.KhuyenMais.FindAsync(id);
            if (khuyenMai == null) return NotFound();

            ChuanHoa(input);
            if (await _context.KhuyenMais.AnyAsync(x => x.MaCode == input.MaCode && x.MaKM != id))
            {
                ModelState.AddModelError(nameof(input.MaCode), "Mã khuyến mãi này đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                input.MaKM = id;
                return View(input);
            }

            khuyenMai.MaCode = input.MaCode;
            khuyenMai.TenKhuyenMai = input.TenKhuyenMai;
            khuyenMai.LoaiGiamGia = input.LoaiGiamGia;
            khuyenMai.GiaTriGiam = input.GiaTriGiam;
            khuyenMai.GiamToiDa = input.GiamToiDa;
            khuyenMai.DonHangToiThieu = input.DonHangToiThieu;
            khuyenMai.NgayBatDau = input.NgayBatDau;
            khuyenMai.NgayKetThuc = input.NgayKetThuc;
            khuyenMai.DangHoatDong = input.DangHoatDong;

            await _context.SaveChangesAsync();
            TempData["success"] = "Đã cập nhật chương trình khuyến mãi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var khuyenMai = await _context.KhuyenMais.FindAsync(id);
            if (khuyenMai == null) return NotFound();

            khuyenMai.DangHoatDong = !khuyenMai.DangHoatDong;
            await _context.SaveChangesAsync();
            TempData["success"] = khuyenMai.DangHoatDong
                ? "Đã kích hoạt chương trình khuyến mãi."
                : "Đã ngừng chương trình khuyến mãi.";
            return RedirectToAction(nameof(Index));
        }

        private static void ChuanHoa(KhuyenMai khuyenMai)
        {
            khuyenMai.MaCode = KhuyenMaiHelper.ChuanHoaMa(khuyenMai.MaCode);
            khuyenMai.NgayBatDau = DateTime.SpecifyKind(khuyenMai.NgayBatDau, DateTimeKind.Utc);
            khuyenMai.NgayKetThuc = DateTime.SpecifyKind(khuyenMai.NgayKetThuc, DateTimeKind.Utc);
        }
    }
}
