using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models;
using BanSach.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [KiemTraVaiTro(VaiTroNguoiDung.Admin)]
    public class ChuDeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChuDeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var query = _context.ChuDes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(x => x.TenChuDe.ToLower().Contains(q.Trim().ToLower()));
            }

            ViewBag.TuKhoa = q;
            var danhSach = await query.OrderByDescending(x => x.MaCD).ToListAsync();
            return View(danhSach);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes
                .Include(x => x.Sachs)
                .FirstOrDefaultAsync(x => x.MaCD == id);

            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChuDe chuDe)
        {
            if (ModelState.IsValid)
            {
                _context.ChuDes.Add(chuDe);
                await _context.SaveChangesAsync();
                TempData["success"] = "Thêm chủ đề thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(chuDe);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes.FindAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChuDe chuDe)
        {
            if (id != chuDe.MaCD)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.ChuDes.Update(chuDe);
                await _context.SaveChangesAsync();
                TempData["success"] = "Cập nhật chủ đề thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(chuDe);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes.FirstOrDefaultAsync(x => x.MaCD == id);
            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chuDe = await _context.ChuDes.Include(x => x.Sachs).FirstOrDefaultAsync(x => x.MaCD == id);
            if (chuDe == null)
            {
                return NotFound();
            }

            if (chuDe.Sachs.Any())
            {
                TempData["error"] = "Không thể xóa chủ đề đang có sách.";
                return RedirectToAction(nameof(Index));
            }

            _context.ChuDes.Remove(chuDe);
            await _context.SaveChangesAsync();
            TempData["success"] = "Xóa chủ đề thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> TaiFileMau()
        {
            var bytes = ChuDeExcelHelper.TaoFileMau(
                await _context.ChuDes.OrderBy(x => x.TenChuDe).Select(x => x.TenChuDe).ToListAsync());
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "mau-nhap-chu-de.xlsx");
        }

        public IActionResult NhapExcel() => View(new NhapChuDeExcelViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> NhapExcel(NhapChuDeExcelViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!Path.GetExtension(model.File!.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.File), "Chỉ hỗ trợ file .xlsx.");
                return View(model);
            }

            KetQuaDocChuDeExcel ketQua;
            await using (var stream = new MemoryStream())
            {
                await model.File.CopyToAsync(stream);
                stream.Position = 0;
                ketQua = ChuDeExcelHelper.DocFile(stream);
            }
            model.Loi.AddRange(ketQua.Loi);

            // So khớp theo tên đã chuẩn hóa (bỏ dấu, chữ thường, bỏ khoảng trắng thừa)
            static string K(string s) => SachExcelHelper.ChuanHoa(s);
            var daCo = (await _context.ChuDes.Select(x => x.TenChuDe).ToListAsync()).Select(K).ToHashSet();
            var trongFile = new Dictionary<string, int>(); // tên chuẩn hóa -> dòng đầu tiên xuất hiện

            var chuDeMois = new List<ChuDe>();
            var boQua = new List<string>();
            foreach (var d in ketQua.Dongs)
            {
                var key = K(d.TenChuDe);
                if (trongFile.TryGetValue(key, out var dongTruoc))
                {
                    if (model.BoQuaTrung) boQua.Add(d.TenChuDe);
                    else model.Loi.Add($"Dòng {d.Dong}: chủ đề \"{d.TenChuDe}\" bị lặp lại với dòng {dongTruoc}.");
                    continue;
                }
                trongFile[key] = d.Dong;

                if (daCo.Contains(key))
                {
                    if (model.BoQuaTrung) boQua.Add(d.TenChuDe);
                    else model.Loi.Add($"Dòng {d.Dong}: chủ đề \"{d.TenChuDe}\" đã tồn tại.");
                    continue;
                }

                chuDeMois.Add(new ChuDe { TenChuDe = d.TenChuDe });
            }

            // Có lỗi thì không nhập gì cả để tránh nhập nửa chừng
            if (model.Loi.Count > 0) return View(model);

            if (chuDeMois.Count == 0)
            {
                model.ThongBao = "Không có chủ đề mới nào để nhập"
                    + (boQua.Count > 0 ? $" ({boQua.Count} chủ đề trùng tên đã được bỏ qua)." : ".");
                return View(model);
            }

            _context.ChuDes.AddRange(chuDeMois);
            await _context.SaveChangesAsync();

            TempData["success"] = $"Đã nhập {chuDeMois.Count} chủ đề từ Excel."
                + (boQua.Count > 0 ? $" Bỏ qua {boQua.Count} chủ đề trùng tên: {string.Join(", ", boQua.Take(5))}{(boQua.Count > 5 ? "..." : "")}." : "");
            return RedirectToAction(nameof(Index));
        }
    }
}
