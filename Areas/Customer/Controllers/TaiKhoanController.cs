using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models;
using BanSach.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<TaiKhoan> _hasher = new();
        private readonly IWebHostEnvironment _env;

        public TaiKhoanController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult DangKy() => View(new DangKyViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var trung = await _context.TaiKhoans.AnyAsync(x =>
                x.TenDangNhap.ToLower() == model.TenDangNhap.Trim().ToLower() ||
                x.Email.ToLower() == model.Email.Trim().ToLower());
            if (trung)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc email đã tồn tại.");
                return View(model);
            }

            var tk = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap.Trim(),
                Email = model.Email.Trim(),
                VaiTro = VaiTroNguoiDung.Customer,
                TrangThai = true
            };
            tk.MatKhauHash = _hasher.HashPassword(tk, model.MatKhau);

            var kh = new KhachHang
            {
                HoTen = model.HoTen.Trim(),
                TaiKhoan = tk
            };

            _context.KhachHangs.Add(kh);
            await _context.SaveChangesAsync();
            GanSession(tk, kh);
            TempData["success"] = "Đăng ký thành công!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult DangNhap(string? returnUrl)
        {
            return View(new DangNhapViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var key = model.TenDangNhap.Trim().ToLower();
            var tk = await _context.TaiKhoans
                .Include(x => x.KhachHang)
                .FirstOrDefaultAsync(x => x.TenDangNhap.ToLower() == key || x.Email.ToLower() == key);

            if (tk == null || _hasher.VerifyHashedPassword(tk, tk.MatKhauHash, model.MatKhau) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Sai tên đăng nhập hoặc mật khẩu.");
                return View(model);
            }

            if (!tk.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa.");
                return View(model);
            }

            GanSession(tk, tk.KhachHang);
            TempData["success"] = "Đăng nhập thành công!";

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            if (tk.VaiTro == VaiTroNguoiDung.Admin)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            TempData["success"] = "Đã đăng xuất.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult QuenMatKhau() => View(new QuenMatKhauViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhauViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var tk = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.Email.ToLower() == model.Email.Trim().ToLower());
            if (tk != null)
            {
                tk.ResetToken = Guid.NewGuid().ToString("N");
                tk.ResetTokenHetHan = DateTime.UtcNow.AddHours(1);
                await _context.SaveChangesAsync();

                if (_env.IsDevelopment())
                {
                    var url = Url.Action("DatLaiMatKhau", "TaiKhoan", new { area = "Customer", token = tk.ResetToken }, Request.Scheme);
                    TempData["success"] = $"Link đặt lại mật khẩu (Development): {url}";
                    return RedirectToAction(nameof(DangNhap));
                }
            }

            TempData["success"] = "Nếu email tồn tại, hệ thống đã tạo yêu cầu đặt lại mật khẩu.";
            return RedirectToAction(nameof(DangNhap));
        }

        [HttpGet]
        public async Task<IActionResult> DatLaiMatKhau(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction(nameof(DangNhap));
            var tk = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.ResetToken == token);
            if (tk == null || tk.ResetTokenHetHan == null || tk.ResetTokenHetHan < DateTime.UtcNow)
            {
                TempData["error"] = "Link đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction(nameof(QuenMatKhau));
            }

            return View(new DatLaiMatKhauViewModel { Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var tk = await _context.TaiKhoans.FirstOrDefaultAsync(x => x.ResetToken == model.Token);
            if (tk == null || tk.ResetTokenHetHan == null || tk.ResetTokenHetHan < DateTime.UtcNow)
            {
                TempData["error"] = "Link đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction(nameof(QuenMatKhau));
            }

            tk.MatKhauHash = _hasher.HashPassword(tk, model.MatKhauMoi);
            tk.ResetToken = null;
            tk.ResetTokenHetHan = null;
            await _context.SaveChangesAsync();
            TempData["success"] = "Đặt lại mật khẩu thành công. Hãy đăng nhập.";
            return RedirectToAction(nameof(DangNhap));
        }

        [KiemTraDangNhap]
        public async Task<IActionResult> HoSo()
        {
            var tk = await TaiKhoanHienTai();
            if (tk == null) return RedirectToAction(nameof(DangNhap));
            return View(new HoSoViewModel
            {
                HoTen = tk.KhachHang?.HoTen ?? tk.TenDangNhap,
                Email = tk.Email,
                TenDangNhap = tk.TenDangNhap
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [KiemTraDangNhap]
        public async Task<IActionResult> HoSo(HoSoViewModel model)
        {
            var tk = await TaiKhoanHienTai();
            if (tk == null) return RedirectToAction(nameof(DangNhap));
            if (!ModelState.IsValid)
            {
                model.TenDangNhap = tk.TenDangNhap;
                return View(model);
            }

            var trungEmail = await _context.TaiKhoans.AnyAsync(x => x.MaTK != tk.MaTK && x.Email.ToLower() == model.Email.Trim().ToLower());
            if (trungEmail)
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");
                model.TenDangNhap = tk.TenDangNhap;
                return View(model);
            }

            tk.Email = model.Email.Trim();
            if (tk.KhachHang != null)
            {
                tk.KhachHang.HoTen = model.HoTen.Trim();
                HttpContext.Session.SetString(SessionKeys.HoTen, tk.KhachHang.HoTen);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction(nameof(HoSo));
        }

        [KiemTraDangNhap]
        public IActionResult DoiMatKhau() => View(new DoiMatKhauViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        [KiemTraDangNhap]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            var tk = await TaiKhoanHienTai();
            if (tk == null) return RedirectToAction(nameof(DangNhap));
            if (!ModelState.IsValid) return View(model);

            if (_hasher.VerifyHashedPassword(tk, tk.MatKhauHash, model.MatKhauHienTai) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(nameof(model.MatKhauHienTai), "Mật khẩu hiện tại không đúng.");
                return View(model);
            }

            tk.MatKhauHash = _hasher.HashPassword(tk, model.MatKhauMoi);
            await _context.SaveChangesAsync();
            TempData["success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(nameof(HoSo));
        }

        private async Task<TaiKhoan?> TaiKhoanHienTai()
        {
            var maTK = HttpContext.Session.GetInt32(SessionKeys.MaTK);
            if (maTK == null) return null;
            return await _context.TaiKhoans.Include(x => x.KhachHang).FirstOrDefaultAsync(x => x.MaTK == maTK);
        }

        private void GanSession(TaiKhoan tk, KhachHang? kh)
        {
            HttpContext.Session.SetInt32(SessionKeys.MaTK, tk.MaTK);
            HttpContext.Session.SetString(SessionKeys.TenDangNhap, tk.TenDangNhap);
            HttpContext.Session.SetString(SessionKeys.VaiTro, tk.VaiTro);
            if (kh != null)
            {
                HttpContext.Session.SetInt32(SessionKeys.MaKH, kh.MaKH);
                HttpContext.Session.SetString(SessionKeys.HoTen, kh.HoTen);
            }
            else
            {
                HttpContext.Session.SetString(SessionKeys.HoTen, tk.TenDangNhap);
            }
        }
    }
}
