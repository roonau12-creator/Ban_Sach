using BanSach.Filters;
using BanSach.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BanSach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [KiemTraVaiTro(VaiTroNguoiDung.Admin)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
