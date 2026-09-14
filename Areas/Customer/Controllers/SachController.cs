using BanSach.Data;
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
            var sach = await _context.Sachs
                .Include(x => x.ChuDe)
                .Include(x => x.NhomSach)
                .Include(x => x.NhaXuatBan)
                .Include(x => x.SachTacGias).ThenInclude(x => x.TacGia)
                .FirstOrDefaultAsync(x => x.MaSach == id);
            if (sach == null) return NotFound();
            return View(sach);
        }
    }
}
