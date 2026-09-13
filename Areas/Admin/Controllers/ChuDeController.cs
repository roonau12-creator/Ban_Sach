using BanSach.Data;
using BanSach.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChuDeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChuDeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ChuDe
        public async Task<IActionResult> Index()
        {
            List<ChuDe> danhSachChuDe = await _context.ChuDes
                .OrderByDescending(x => x.MaCD)
                .ToListAsync();

            return View(danhSachChuDe);
        }

        // GET: Admin/ChuDe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ChuDe? chuDe = await _context.ChuDes
                .FirstOrDefaultAsync(x => x.MaCD == id);

            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        // GET: Admin/ChuDe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ChuDe/Create
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

        // GET: Admin/ChuDe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ChuDe? chuDe = await _context.ChuDes.FindAsync(id);

            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        // POST: Admin/ChuDe/Edit/5
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

        // GET: Admin/ChuDe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ChuDe? chuDe = await _context.ChuDes
                .FirstOrDefaultAsync(x => x.MaCD == id);

            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        // POST: Admin/ChuDe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ChuDe? chuDe = await _context.ChuDes.FindAsync(id);

            if (chuDe == null)
            {
                return NotFound();
            }

            _context.ChuDes.Remove(chuDe);
            await _context.SaveChangesAsync();

            TempData["success"] = "Xóa chủ đề thành công!";

            return RedirectToAction(nameof(Index));
        }
    }
}