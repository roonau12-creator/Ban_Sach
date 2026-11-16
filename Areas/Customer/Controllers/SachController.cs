using BanSach.Data;
using BanSach.Helpers;
using BanSach.Models;
using BanSach.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SachController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SachController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q, int? maCD, int? maNhom, int? maNXB)
        {
            var query = _context.Sachs
                .Include(x => x.ChuDe)
                .Include(x => x.NhomSach)
                .Include(x => x.NhaXuatBan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(x => x.TenSach.ToLower().Contains(q.Trim().ToLower()));
            }

            if (maCD.HasValue) query = query.Where(x => x.MaCD == maCD);
            if (maNhom.HasValue) query = query.Where(x => x.MaNhom == maNhom);
            if (maNXB.HasValue) query = query.Where(x => x.MaNXB == maNXB);

            ViewBag.TuKhoa = q;
            ViewBag.MaCD = maCD;
            ViewBag.MaNhom = maNhom;
            ViewBag.MaNXB = maNXB;
            ViewBag.ChuDes = new SelectList(await _context.ChuDes.OrderBy(x => x.TenChuDe).ToListAsync(), "MaCD", "TenChuDe", maCD);
            ViewBag.NhomSachs = new SelectList(
                await _context.NhomSachs.Where(x => x.TrangThai).OrderBy(x => x.ThuTu).ToListAsync(),
                "MaNhom", "TenNhom", maNhom);

            return View(await query.OrderByDescending(x => x.MaSach).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var model = await TaoChiTietSach(id.Value, HttpContext.Session.GetInt32(SessionKeys.MaTK));
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuiDanhGia(int id, [Bind(Prefix = "DanhGiaMoi")] DanhGiaFormViewModel form)
        {
            if (!await _context.Sachs.AnyAsync(x => x.MaSach == id)) return NotFound();

            var maTK = HttpContext.Session.GetInt32(SessionKeys.MaTK);
            if (maTK == null)
            {
                var returnUrl = Url.Action(nameof(Details), "Sach", new { area = "Customer", id });
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "Customer", returnUrl });
            }

            var laKhachHang = await _context.TaiKhoans.AnyAsync(x =>
                x.MaTK == maTK &&
                x.VaiTro == VaiTroNguoiDung.Customer &&
                x.TrangThai &&
                x.KhachHang != null);
            if (!laKhachHang)
            {
                TempData["error"] = "Chỉ khách hàng mới có thể gửi đánh giá.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var daMuaVaNhanSach = await _context.DonHangs.AnyAsync(x =>
                x.KhachHang!.MaTK == maTK &&
                x.TrangThai == TrangThaiDonHang.DaGiao &&
                x.ChiTietDonHangs.Any(ct => ct.MaSach == id));
            if (!daMuaVaNhanSach)
            {
                TempData["error"] = "Bạn chỉ có thể đánh giá sách đã mua và nhận hàng.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (await _context.DanhGias.AnyAsync(x => x.MaTK == maTK && x.MaSach == id))
            {
                TempData["error"] = "Bạn đã đánh giá sách này.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (!ModelState.IsValid)
            {
                var invalidModel = await TaoChiTietSach(id, maTK, form);
                if (invalidModel == null) return NotFound();
                return View("Details", invalidModel);
            }

            var noiDung = form.NoiDung.Trim();
            if (noiDung.Length < 5)
            {
                ModelState.AddModelError("DanhGiaMoi.NoiDung", "Nhận xét phải có từ 5 đến 2000 ký tự.");
                var invalidModel = await TaoChiTietSach(id, maTK, form);
                if (invalidModel == null) return NotFound();
                return View("Details", invalidModel);
            }

            var danhGia = new DanhGia
            {
                MaTK = maTK.Value,
                MaSach = id,
                SoSao = form.SoSao,
                NoiDung = noiDung,
                NgayDanhGia = DateTime.UtcNow
            };
            _context.DanhGias.Add(danhGia);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (await _context.DanhGias.AnyAsync(x => x.MaTK == maTK && x.MaSach == id))
                {
                    TempData["error"] = "Bạn đã đánh giá sách này.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                throw;
            }

            TempData["success"] = "Đã gửi đánh giá.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<ChiTietSachViewModel?> TaoChiTietSach(
            int maSach,
            int? maTK,
            DanhGiaFormViewModel? form = null)
        {
            var sach = await _context.Sachs
                .AsNoTracking()
                .Include(x => x.ChuDe)
                .Include(x => x.NhomSach)
                .Include(x => x.NhaXuatBan)
                .Include(x => x.SachTacGias).ThenInclude(x => x.TacGia)
                .FirstOrDefaultAsync(x => x.MaSach == maSach);
            if (sach == null) return null;

            var danhGias = await _context.DanhGias
                .AsNoTracking()
                .Where(x => x.MaSach == maSach && !x.An)
                .Include(x => x.TaiKhoan)
                .OrderByDescending(x => x.NgayDanhGia)
                .ToListAsync();

            var daDanhGia = maTK.HasValue && await _context.DanhGias
                .AnyAsync(x => x.MaTK == maTK.Value && x.MaSach == maSach);
            var laKhachHang = maTK.HasValue && await _context.TaiKhoans.AnyAsync(x =>
                x.MaTK == maTK.Value &&
                x.VaiTro == VaiTroNguoiDung.Customer &&
                x.TrangThai &&
                x.KhachHang != null);
            var daMuaVaNhanSach = laKhachHang && await _context.DonHangs.AnyAsync(x =>
                x.KhachHang!.MaTK == maTK.GetValueOrDefault() &&
                x.TrangThai == TrangThaiDonHang.DaGiao &&
                x.ChiTietDonHangs.Any(ct => ct.MaSach == maSach));

            return new ChiTietSachViewModel
            {
                Sach = sach,
                DanhGias = danhGias,
                DiemTrungBinh = danhGias.Count == 0 ? 0 : danhGias.Average(x => x.SoSao),
                DaDangNhap = maTK.HasValue,
                DaDanhGia = daDanhGia,
                CoTheDanhGia = daMuaVaNhanSach && !daDanhGia,
                DanhGiaMoi = form ?? new DanhGiaFormViewModel()
            };
        }
    }
}
