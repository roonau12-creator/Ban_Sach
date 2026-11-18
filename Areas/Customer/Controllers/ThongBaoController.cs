using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    [KiemTraDangNhap]
    public class ThongBaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongBaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var maTK = HttpContext.Session.GetInt32(SessionKeys.MaTK)!.Value;
            var thongBaos = await _context.ThongBaos
                .Where(x => x.MaTK == maTK && !x.An)
                .OrderByDescending(x => x.NgayTao)
                .ToListAsync();
            return View(thongBaos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhDauDaDoc(int id)
        {
            var maTK = HttpContext.Session.GetInt32(SessionKeys.MaTK)!.Value;
            var thongBao = await _context.ThongBaos
                .FirstOrDefaultAsync(x => x.MaTB == id && x.MaTK == maTK && !x.An);
            if (thongBao == null) return NotFound();

            thongBao.DaDoc = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhDauTatCaDaDoc()
        {
            var maTK = HttpContext.Session.GetInt32(SessionKeys.MaTK)!.Value;
            var thongBaos = await _context.ThongBaos
                .Where(x => x.MaTK == maTK && !x.An && !x.DaDoc)
                .ToListAsync();

            foreach (var thongBao in thongBaos)
            {
                thongBao.DaDoc = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> An(int id)
        {
            var maTK = HttpContext.Session.GetInt32(SessionKeys.MaTK)!.Value;
            var thongBao = await _context.ThongBaos
                .FirstOrDefaultAsync(x => x.MaTB == id && x.MaTK == maTK && !x.An);
            if (thongBao == null) return NotFound();

            thongBao.An = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
