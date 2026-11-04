using BanSach.Data;
using BanSach.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = AuthorizationPolicies.ManageBooks)]
    public class ChuDeController : Controller
    {
        private const string DuplicateNameIndex = "IX_ChuDes_TenChuDeNormalized";
        private readonly ApplicationDbContext _context;

        public ChuDeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<ChuDe> categories = await _context.ChuDes
                .Include(category => category.Sachs)
                .OrderBy(category => category.TenChuDe)
                .ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ChuDe? category = await _context.ChuDes
                .Include(item => item.Sachs)
                .FirstOrDefaultAsync(item => item.MaCD == id);

            return category == null ? NotFound() : View(category);
        }

        public IActionResult Create()
        {
            return View(new ChuDe());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChuDe category)
        {
            category.MaCD = 0;
            category.Sachs = new List<Sach>();
            NormalizeInput(category);
            if (ModelState.IsValid &&
                await HasDuplicateNameAsync(category.TenChuDeNormalized!, null))
            {
                ModelState.AddModelError(nameof(category.TenChuDe), "Tên danh mục này đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _context.ChuDes.Add(category);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (IsDuplicateNameException(exception))
            {
                ModelState.AddModelError(nameof(category.TenChuDe), "Tên danh mục này đã tồn tại.");
                return View(category);
            }

            TempData["success"] = "Thêm danh mục thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ChuDe? category = await _context.ChuDes.FindAsync(id);
            return category == null ? NotFound() : View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChuDe category)
        {
            if (id != category.MaCD)
            {
                return NotFound();
            }

            NormalizeInput(category);
            if (ModelState.IsValid &&
                await HasDuplicateNameAsync(category.TenChuDeNormalized!, id))
            {
                ModelState.AddModelError(nameof(category.TenChuDe), "Tên danh mục này đã tồn tại.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            ChuDe? existingCategory = await _context.ChuDes.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.TenChuDe = category.TenChuDe;
            existingCategory.MoTa = category.MoTa;
            existingCategory.TenChuDeNormalized = category.TenChuDeNormalized;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (IsDuplicateNameException(exception))
            {
                ModelState.AddModelError(nameof(category.TenChuDe), "Tên danh mục này đã tồn tại.");
                return View(category);
            }

            TempData["success"] = "Cập nhật danh mục thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ChuDe? category = await _context.ChuDes
                .Include(item => item.Sachs)
                .FirstOrDefaultAsync(item => item.MaCD == id);

            return category == null ? NotFound() : View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ChuDe? category = await _context.ChuDes
                .Include(item => item.Sachs)
                .FirstOrDefaultAsync(item => item.MaCD == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Sachs.Count > 0)
            {
                ViewBag.DeleteError = "Không thể xóa danh mục đang có sách. Hãy chuyển các sách sang danh mục khác trước.";
                return View("Delete", category);
            }

            _context.ChuDes.Remove(category);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (IsReferencedCategoryException(exception))
            {
                _context.Entry(category).State = EntityState.Unchanged;
                category.Sachs = await _context.Sachs
                    .AsNoTracking()
                    .Where(book => book.MaCD == id)
                    .OrderBy(book => book.TenSach)
                    .ToListAsync();
                ViewBag.DeleteError = "Danh mục vừa được gán cho sách khác nên không thể xóa. Hãy chuyển các sách sang danh mục khác trước.";
                return View("Delete", category);
            }

            TempData["success"] = "Xóa danh mục thành công.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> HasDuplicateNameAsync(string normalizedName, int? excludedId)
        {
            var existingNames = await _context.ChuDes
                .AsNoTracking()
                .Where(category => excludedId == null || category.MaCD != excludedId)
                .Select(category => new { category.TenChuDe, category.TenChuDeNormalized })
                .ToListAsync();

            return existingNames.Any(category =>
                string.Equals(category.TenChuDeNormalized, normalizedName, StringComparison.Ordinal) ||
                string.Equals(Normalize(category.TenChuDe), normalizedName, StringComparison.Ordinal));
        }

        private static void NormalizeInput(ChuDe category)
        {
            category.TenChuDe = category.TenChuDe?.Trim() ?? string.Empty;
            category.MoTa = string.IsNullOrWhiteSpace(category.MoTa) ? null : category.MoTa.Trim();
            category.TenChuDeNormalized = Normalize(category.TenChuDe);
        }

        private static string Normalize(string name)
        {
            return name.Trim().ToUpperInvariant();
        }

        private static bool IsDuplicateNameException(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: DuplicateNameIndex
            };
        }

        private static bool IsReferencedCategoryException(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.ForeignKeyViolation
            };
        }
    }
}
