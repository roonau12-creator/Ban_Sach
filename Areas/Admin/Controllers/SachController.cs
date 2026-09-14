using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models;
using BanSach.Models.ViewModels;
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

        public SachController(ApplicationDbContext context, AnhBiaHelper anhBia)
        {
            _context = context;
            _anhBia = anhBia;
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
