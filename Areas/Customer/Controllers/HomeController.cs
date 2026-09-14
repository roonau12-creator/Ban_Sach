using System.Diagnostics;
using BanSach.Data;
using BanSach.Models;
using BanSach.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                SachMois = await _context.Sachs
                    .Include(x => x.ChuDe)
                    .Where(x => x.SachMoi)
                    .OrderByDescending(x => x.MaSach)
                    .Take(8)
                    .ToListAsync(),
                SachNoiBats = await _context.Sachs
                    .Include(x => x.ChuDe)
                    .Where(x => x.NoiBat)
                    .OrderByDescending(x => x.MaSach)
                    .Take(8)
                    .ToListAsync(),
                ChuDes = await _context.ChuDes.OrderBy(x => x.TenChuDe).ToListAsync()
            };

            if (!model.SachMois.Any())
            {
                model.SachMois = await _context.Sachs.Include(x => x.ChuDe)
                    .OrderByDescending(x => x.MaSach).Take(8).ToListAsync();
            }

            if (!model.SachNoiBats.Any())
            {
                model.SachNoiBats = await _context.Sachs.Include(x => x.ChuDe)
                    .OrderByDescending(x => x.GiaBan).Take(8).ToListAsync();
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
