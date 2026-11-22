using System.Data;
using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models;
using BanSach.Models.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [KiemTraVaiTro(VaiTroNguoiDung.Admin)]
    public class SachController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AnhBiaHelper _anhBia;
        private readonly SachExcelService _sachExcel;
        private readonly IMemoryCache _memoryCache;

        public SachController(
            ApplicationDbContext context,
            AnhBiaHelper anhBia,
            SachExcelService sachExcel,
            IMemoryCache memoryCache)
        {
            _context = context;
            _anhBia = anhBia;
            _sachExcel = sachExcel;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(string? q, int? maCD, int? maNXB, int? maNhom)
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
            if (maNXB.HasValue) query = query.Where(x => x.MaNXB == maNXB);
            if (maNhom.HasValue) query = query.Where(x => x.MaNhom == maNhom);

            ViewBag.TuKhoa = q;
            ViewBag.MaCD = maCD;
            ViewBag.MaNXB = maNXB;
            ViewBag.MaNhom = maNhom;
            ViewBag.ChuDes = new SelectList(await _context.ChuDes.OrderBy(x => x.TenChuDe).ToListAsync(), "MaCD", "TenChuDe", maCD);
            ViewBag.NhaXuatBans = new SelectList(await _context.NhaXuatBans.OrderBy(x => x.TenNXB).ToListAsync(), "MaNXB", "TenNXB", maNXB);
            ViewBag.NhomSachs = new SelectList(await _context.NhomSachs.OrderBy(x => x.ThuTu).ToListAsync(), "MaNhom", "TenNhom", maNhom);

            return View(await query.OrderByDescending(x => x.MaSach).ToListAsync());
        }

        [HttpGet]
        public IActionResult NhapExcel()
        {
            return View(new SachExcelImportPreviewViewModel());
        }

        [HttpGet]
        public IActionResult TaiMauNhapExcel()
        {
            return File(
                _sachExcel.TaoMauNhap(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "MauNhapSach.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> XuatExcel(int? maCD, string? tinhTrang)
        {
            var query = _context.Sachs
                .AsNoTracking()
                .Include(x => x.ChuDe)
                .Include(x => x.NhomSach)
                .Include(x => x.NhaXuatBan)
                .Include(x => x.SachTacGias)
                    .ThenInclude(x => x.TacGia)
                .AsQueryable();

            if (maCD.HasValue)
            {
                query = query.Where(x => x.MaCD == maCD.Value);
            }

            switch (tinhTrang)
            {
                case null:
                case "":
                    break;
                case "ConHang":
                    query = query.Where(x => x.SoLuongTon > 0);
                    break;
                case "HetHang":
                    query = query.Where(x => x.SoLuongTon == 0);
                    break;
                case "SachMoi":
                    query = query.Where(x => x.SachMoi);
                    break;
                case "NoiBat":
                    query = query.Where(x => x.NoiBat);
                    break;
                default:
                    return BadRequest("Tình trạng xuất Excel không hợp lệ.");
            }

            var sachs = await query.OrderBy(x => x.MaSach).ToListAsync();
            return File(
                _sachExcel.XuatExcel(sachs),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"DanhSachSach_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> XemTruocExcel(IFormFile? file)
        {
            var preview = new SachExcelImportPreviewViewModel();
            if (file == null || file.Length == 0)
            {
                preview.FileErrors.Add("Vui lòng chọn file Excel có dữ liệu.");
                return View("NhapExcel", preview);
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                preview.FileErrors.Add("File Excel không được vượt quá 5 MB.");
                return View("NhapExcel", preview);
            }

            if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                preview.FileErrors.Add("Chỉ chấp nhận file Excel có định dạng .xlsx.");
                return View("NhapExcel", preview);
            }

            await using var stream = file.OpenReadStream();
            preview = _sachExcel.DocNhap(stream);
            if (preview.FileErrors.Count == 0 && preview.Rows.Count > 0)
            {
                await KiemTraDuLieuNhapAsync(preview);
            }

            if (preview.CanConfirm)
            {
                preview.Token = Guid.NewGuid().ToString("N");
                _memoryCache.Set(
                    TaoKhoaCacheNhapExcel(HttpContext.Session.GetInt32(SessionKeys.MaTK)!.Value, preview.Token),
                    preview,
                    TimeSpan.FromMinutes(20));
            }

            return View("NhapExcel", preview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhanNhapExcel(string token)
        {
            var maTK = HttpContext.Session.GetInt32(SessionKeys.MaTK)!.Value;
            if (!Guid.TryParseExact(token, "N", out _))
            {
                return NotFound();
            }

            var cacheKey = TaoKhoaCacheNhapExcel(maTK, token);
            if (!_memoryCache.TryGetValue<SachExcelImportPreviewViewModel>(cacheKey, out var preview) || preview == null)
            {
                TempData["error"] = "Phiên xem trước đã hết hạn. Vui lòng tải lại file Excel.";
                return RedirectToAction(nameof(NhapExcel));
            }

            if (!preview.CanConfirm)
            {
                return NotFound();
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            await KiemTraDuLieuNhapAsync(preview);
            if (!preview.CanConfirm)
            {
                _memoryCache.Remove(cacheKey);
                return View("NhapExcel", preview);
            }

            _context.Sachs.AddRange(preview.Rows.Select(x => x.ToEntity()));
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _memoryCache.Remove(cacheKey);

            TempData["success"] = $"Đã nhập thành công {preview.Rows.Count} sách.";
            return RedirectToAction(nameof(Index));
        }

        private async Task KiemTraDuLieuNhapAsync(SachExcelImportPreviewViewModel preview)
        {
            var tenSachs = await _context.Sachs
                .AsNoTracking()
                .Select(x => x.TenSach)
                .ToListAsync();
            var tenSachDaCo = new HashSet<string>(
                tenSachs.Select(x => x.Trim()),
                StringComparer.OrdinalIgnoreCase);
            var maCDs = (await _context.ChuDes.Select(x => x.MaCD).ToListAsync()).ToHashSet();
            var maNhoms = (await _context.NhomSachs.Select(x => x.MaNhom).ToListAsync()).ToHashSet();
            var maNXBs = (await _context.NhaXuatBans.Select(x => x.MaNXB).ToListAsync()).ToHashSet();
            var maTGs = (await _context.TacGias.Select(x => x.MaTG).ToListAsync()).ToHashSet();

            foreach (var row in preview.Rows)
            {
                if (!string.IsNullOrWhiteSpace(row.TenSach) && !tenSachDaCo.Add(row.TenSach.Trim()))
                {
                    row.Errors.Add($"Dòng {row.RowNumber}: Tên sách bị trùng với sách hiện có hoặc dòng khác trong file.");
                }

                if (row.MaCD.HasValue && !maCDs.Contains(row.MaCD.Value))
                {
                    row.Errors.Add($"Dòng {row.RowNumber}: Mã chủ đề {row.MaCD.Value} không tồn tại.");
                }

                if (row.MaNhom.HasValue && !maNhoms.Contains(row.MaNhom.Value))
                {
                    row.Errors.Add($"Dòng {row.RowNumber}: Mã nhóm {row.MaNhom.Value} không tồn tại.");
                }

                if (row.MaNXB.HasValue && !maNXBs.Contains(row.MaNXB.Value))
                {
                    row.Errors.Add($"Dòng {row.RowNumber}: Mã nhà xuất bản {row.MaNXB.Value} không tồn tại.");
                }

                foreach (var maTG in row.MaTacGias.Where(x => !maTGs.Contains(x)))
                {
                    row.Errors.Add($"Dòng {row.RowNumber}: Mã tác giả {maTG} không tồn tại.");
                }
            }
        }

        private static string TaoKhoaCacheNhapExcel(int maTK, string token) =>
            $"sach-excel-import:{maTK}:{token}";

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var sach = await _context.Sachs
                .Include(x => x.ChuDe)
                .Include(x => x.NhomSach)
                .Include(x => x.NhaXuatBan)
                .Include(x => x.SachTacGias).ThenInclude(x => x.TacGia)
                .FirstOrDefaultAsync(x => x.MaSach == id);
            if (sach == null) return NotFound();
            return View(sach);
        }

        public async Task<IActionResult> Create()
        {
            return View(await TaoForm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SachFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await NapDanhMuc(model);
                return View(model);
            }

            try
            {
                var anh = await _anhBia.LuuAnh(model.AnhBiaFile);
                var sach = MapSach(model);
                sach.AnhBia = anh;
                _context.Sachs.Add(sach);
                await _context.SaveChangesAsync();
                await CapNhatTacGia(sach.MaSach, model.MaTacGias);
                TempData["success"] = "Thêm sách thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.AnhBiaFile), ex.Message);
                await NapDanhMuc(model);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var sach = await _context.Sachs.Include(x => x.SachTacGias).FirstOrDefaultAsync(x => x.MaSach == id);
            if (sach == null) return NotFound();
            var model = await TaoForm(sach);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SachFormViewModel model)
        {
            if (id != model.MaSach) return NotFound();
            if (!ModelState.IsValid)
            {
                await NapDanhMuc(model);
                return View(model);
            }

            var sach = await _context.Sachs.Include(x => x.SachTacGias).FirstOrDefaultAsync(x => x.MaSach == id);
            if (sach == null) return NotFound();

            try
            {
                if (model.AnhBiaFile != null)
                {
                    _anhBia.XoaAnh(sach.AnhBia);
                    sach.AnhBia = await _anhBia.LuuAnh(model.AnhBiaFile);
                }

                sach.TenSach = model.TenSach;
                sach.MaCD = model.MaCD;
                sach.MaNhom = model.MaNhom;
                sach.MaNXB = model.MaNXB;
                sach.GiaBan = model.GiaBan;
                sach.SoLuongTon = model.SoLuongTon;
                sach.MoTa = model.MoTa;
                sach.NamXB = model.NamXB;
                sach.SachMoi = model.SachMoi;
                sach.NoiBat = model.NoiBat;
                await _context.SaveChangesAsync();
                await CapNhatTacGia(sach.MaSach, model.MaTacGias);
                TempData["success"] = "Cập nhật sách thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.AnhBiaFile), ex.Message);
                await NapDanhMuc(model);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var sach = await _context.Sachs
                .Include(x => x.ChuDe)
                .FirstOrDefaultAsync(x => x.MaSach == id);
            if (sach == null) return NotFound();
            return View(sach);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Sachs.Include(x => x.ChiTietDonHangs).FirstOrDefaultAsync(x => x.MaSach == id);
            if (sach == null) return NotFound();
            if (sach.ChiTietDonHangs.Any())
            {
                TempData["error"] = "Không thể xóa sách đã phát sinh đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            _anhBia.XoaAnh(sach.AnhBia);
            _context.Sachs.Remove(sach);
            await _context.SaveChangesAsync();
            TempData["success"] = "Xóa sách thành công!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<SachFormViewModel> TaoForm(Sach? sach = null)
        {
            var model = sach == null
                ? new SachFormViewModel()
                : new SachFormViewModel
                {
                    MaSach = sach.MaSach,
                    TenSach = sach.TenSach,
                    MaCD = sach.MaCD,
                    MaNhom = sach.MaNhom,
                    MaNXB = sach.MaNXB,
                    GiaBan = sach.GiaBan,
                    SoLuongTon = sach.SoLuongTon,
                    AnhBia = sach.AnhBia,
                    MoTa = sach.MoTa,
                    NamXB = sach.NamXB,
                    SachMoi = sach.SachMoi,
                    NoiBat = sach.NoiBat,
                    MaTacGias = sach.SachTacGias.Select(x => x.MaTG).ToList()
                };
            await NapDanhMuc(model);
            return model;
        }

        private async Task NapDanhMuc(SachFormViewModel model)
        {
            model.ChuDes = new SelectList(await _context.ChuDes.OrderBy(x => x.TenChuDe).ToListAsync(), "MaCD", "TenChuDe", model.MaCD);
            model.NhomSachs = new SelectList(await _context.NhomSachs.OrderBy(x => x.ThuTu).ToListAsync(), "MaNhom", "TenNhom", model.MaNhom);
            model.NhaXuatBans = new SelectList(await _context.NhaXuatBans.OrderBy(x => x.TenNXB).ToListAsync(), "MaNXB", "TenNXB", model.MaNXB);
            model.TacGias = await _context.TacGias.OrderBy(x => x.TenTacGia)
                .Select(x => new SelectListItem
                {
                    Value = x.MaTG.ToString(),
                    Text = x.TenTacGia,
                    Selected = model.MaTacGias.Contains(x.MaTG)
                }).ToListAsync();
        }

        private static Sach MapSach(SachFormViewModel model) => new()
        {
            TenSach = model.TenSach,
            MaCD = model.MaCD,
            MaNhom = model.MaNhom,
            MaNXB = model.MaNXB,
            GiaBan = model.GiaBan,
            SoLuongTon = model.SoLuongTon,
            MoTa = model.MoTa,
            NamXB = model.NamXB,
            SachMoi = model.SachMoi,
            NoiBat = model.NoiBat
        };

        private async Task CapNhatTacGia(int maSach, List<int> maTacGias)
        {
            var cu = await _context.SachTacGias.Where(x => x.MaSach == maSach).ToListAsync();
            _context.SachTacGias.RemoveRange(cu);
            foreach (var maTG in maTacGias.Distinct())
            {
                _context.SachTacGias.Add(new SachTacGia { MaSach = maSach, MaTG = maTG });
            }

            await _context.SaveChangesAsync();
        }
    }
}
