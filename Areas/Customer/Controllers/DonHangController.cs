using BanSach.Data;
using BanSach.Filters;
using BanSach.Helpers;
using BanSach.Models;
using BanSach.Models.ViewModels;
using BanSach.Services.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BanSach.Areas.Customer.Controllers
{
    [Area("Customer")]
    [KiemTraDangNhap]
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionGioHang _gioHang;
        private readonly GioHangService _gioHangService;
        private readonly ILogger<DonHangController> _logger;

        public DonHangController(
            ApplicationDbContext context,
            SessionGioHang gioHang,
            GioHangService gioHangService,
            ILogger<DonHangController> logger)
        {
            _context = context;
            _gioHang = gioHang;
            _gioHangService = gioHangService;
            _logger = logger;
        }

        public async Task<IActionResult> DatHang()
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null)
            {
                TempData["error"] = "Tài khoản quản trị không đặt hàng từ giỏ khách. Hãy dùng tài khoản khách hàng.";
                return RedirectToAction("Index", "GioHang");
            }

            if (await _gioHangService.DongBoAsync())
            {
                TempData["error"] = "Giỏ hàng đã được cập nhật theo giá và tồn kho hiện tại. Vui lòng kiểm tra lại.";
            }

            var (items, error) = await LayGioHangXacNhanAsync();
            if (error != null)
            {
                TempData["error"] = error;
                return RedirectToAction("Index", "GioHang");
            }

            var diaChis = await _context.DiaChis.Where(x => x.MaKH == maKH).ToListAsync();
            var sdts = await _context.SoDienThoais.Where(x => x.MaKH == maKH).ToListAsync();
            if (!diaChis.Any() || !sdts.Any())
            {
                TempData["error"] = "Hãy thêm ít nhất một địa chỉ và một số điện thoại trước khi đặt hàng.";
                return RedirectToAction("Index", !diaChis.Any() ? "DiaChi" : "SoDienThoai");
            }

            return View(TaoDatHangViewModel(items, diaChis, sdts));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatHang(DatHangViewModel model, string? hanhDong)
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null) return RedirectToAction("DangNhap", "TaiKhoan");

            if (model.MaYeuCau == Guid.Empty)
            {
                ModelState.AddModelError(nameof(model.MaYeuCau), "Yêu cầu đặt hàng không hợp lệ. Vui lòng tải lại trang.");
            }
            else
            {
                var previousOrderId = await _context.DonHangs
                    .AsNoTracking()
                    .Where(x => x.MaYeuCau == model.MaYeuCau)
                    .Select(x => new { x.MaDH, x.MaKH })
                    .FirstOrDefaultAsync();
                if (previousOrderId != null)
                {
                    if (previousOrderId.MaKH != maKH.Value) return BadRequest();
                    _gioHang.XoaGio();
                    return RedirectToAction(nameof(ChiTiet), new { id = previousOrderId.MaDH });
                }
            }

            if (await _gioHangService.DongBoAsync())
            {
                TempData["error"] = "Giá hoặc tồn kho đã thay đổi. Vui lòng kiểm tra lại giỏ hàng trước khi đặt.";
                return RedirectToAction("Index", "GioHang");
            }

            var (items, error) = await LayGioHangXacNhanAsync();
            if (error != null)
            {
                TempData["error"] = error;
                return RedirectToAction("Index", "GioHang");
            }

            var diaChis = await _context.DiaChis.Where(x => x.MaKH == maKH).ToListAsync();
            var sdts = await _context.SoDienThoais.Where(x => x.MaKH == maKH).ToListAsync();
            if (!diaChis.Any() || !sdts.Any())
            {
                TempData["error"] = "Hãy thêm ít nhất một địa chỉ và một số điện thoại trước khi đặt hàng.";
                return RedirectToAction("Index", !diaChis.Any() ? "DiaChi" : "SoDienThoai");
            }

            model.Items = items;
            model.TongHang = items.Sum(x => x.ThanhTien);
            model.GiamGia = 0;
            model.TongTien = model.TongHang;
            model.DiaChis = new SelectList(diaChis, "MaDC", "DiaChiChiTiet", model.MaDC);
            model.SoDienThoais = new SelectList(sdts, "MaSDT", "SoDT", model.MaSDT);

            if (hanhDong == "ap-dung")
            {
                var (khuyenMai, loi, giamGia) = await KiemTraKhuyenMaiAsync(model.MaKhuyenMai, model.TongHang, true);
                if (loi != null)
                {
                    ModelState.AddModelError(nameof(model.MaKhuyenMai), loi);
                }
                else if (khuyenMai != null)
                {
                    model.GiamGia = giamGia;
                    model.TongTien = model.TongHang - giamGia;
                }

                return View(model);
            }

            if (!diaChis.Any(x => x.MaDC == model.MaDC))
                ModelState.AddModelError(nameof(model.MaDC), "Địa chỉ giao hàng không hợp lệ.");
            if (!sdts.Any(x => x.MaSDT == model.MaSDT))
                ModelState.AddModelError(nameof(model.MaSDT), "Số điện thoại không hợp lệ.");
            if (model.PhuongThucThanhToan != PhuongThucThanhToan.COD
                && model.PhuongThucThanhToan != PhuongThucThanhToan.ChuyenKhoan)
                ModelState.AddModelError(nameof(model.PhuongThucThanhToan), "Phương thức thanh toán không hợp lệ.");

            if (!ModelState.IsValid)
            {
                return View(TaoDatHangViewModel(items, diaChis, sdts, model));
            }

            var (maKhuyenMai, loiKhuyenMai, soTienGiam) =
                await KiemTraKhuyenMaiAsync(model.MaKhuyenMai, model.TongHang, false);
            if (loiKhuyenMai != null)
            {
                ModelState.AddModelError(nameof(model.MaKhuyenMai), loiKhuyenMai);
                return View(model);
            }

            var diaChi = diaChis.First(x => x.MaDC == model.MaDC);
            var sdt = sdts.First(x => x.MaSDT == model.MaSDT);
            var khachHang = await _context.KhachHangs
                .Include(x => x.TaiKhoan)
                .FirstOrDefaultAsync(x => x.MaKH == maKH);
            if (khachHang?.TaiKhoan == null)
            {
                _logger.LogError("Customer account {CustomerId} was not found while placing an order.", maKH);
                TempData["error"] = "Không thể xác định tài khoản khách hàng để gửi xác nhận đơn hàng.";
                return RedirectToAction("Index", "GioHang");
            }
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var maSachs = items.Select(x => x.MaSach).OrderBy(x => x).ToArray();
                var sachTrongKho = await _context.Sachs
                    .FromSqlInterpolated(
                        $"SELECT * FROM \"Sachs\" WHERE \"MaSach\" = ANY ({maSachs}) ORDER BY \"MaSach\" FOR UPDATE")
                    .ToListAsync();
                var previousOrderId = await _context.DonHangs
                    .AsNoTracking()
                    .Where(x => x.MaYeuCau == model.MaYeuCau)
                    .Select(x => new { x.MaDH, x.MaKH })
                    .FirstOrDefaultAsync();
                if (previousOrderId != null)
                {
                    await tx.RollbackAsync();
                    if (previousOrderId.MaKH != maKH.Value) return BadRequest();
                    _gioHang.XoaGio();
                    return RedirectToAction(nameof(ChiTiet), new { id = previousOrderId.MaDH });
                }

                if (sachTrongKho.Count != items.Count)
                {
                    await tx.RollbackAsync();
                    TempData["error"] = "Một hoặc nhiều sách trong giỏ hàng không còn tồn tại.";
                    return RedirectToAction("Index", "GioHang");
                }

                var sachTheoMa = sachTrongKho.ToDictionary(x => x.MaSach);
                var sachKhongDuTon = items.FirstOrDefault(
                    x => sachTheoMa[x.MaSach].SoLuongTon < x.SoLuong);
                if (sachKhongDuTon != null)
                {
                    await tx.RollbackAsync();
                    TempData["error"] = $"Sách \"{sachTheoMa[sachKhongDuTon.MaSach].TenSach}\" không đủ số lượng tồn.";
                    return RedirectToAction("Index", "GioHang");
                }

                var tongHang = items.Sum(x => sachTheoMa[x.MaSach].GiaBan * x.SoLuong);
                var don = new DonHang
                {
                    MaKH = maKH.Value,
                    NgayDat = DateTime.UtcNow,
                    TrangThai = TrangThaiDonHang.ChoXacNhan,
                    MaYeuCau = model.MaYeuCau,
                    TongTien = tongHang - soTienGiam,
                    SoTienGiam = soTienGiam,
                    MaKM = maKhuyenMai?.MaKM,
                    MaKhuyenMai = maKhuyenMai?.MaCode,
                    PhuongThucThanhToan = model.PhuongThucThanhToan,
                    TrangThaiThanhToan = model.PhuongThucThanhToan == PhuongThucThanhToan.ChuyenKhoan
                        ? TrangThaiThanhToan.DaThanhToan
                        : TrangThaiThanhToan.ChuaThanhToan,
                    MaDC = diaChi.MaDC,
                    MaSDT = sdt.MaSDT,
                    DiaChiGiao = diaChi.DiaChiChiTiet,
                    SoDienThoaiGiao = sdt.SoDT,
                    GhiChu = model.GhiChu,
                    ChiTietDonHangs = items.Select(x => new ChiTietDonHang
                    {
                        MaSach = x.MaSach,
                        SoLuong = x.SoLuong,
                        DonGia = sachTheoMa[x.MaSach].GiaBan
                    }).ToList(),
                    GiaoHang = new GiaoHang
                    {
                        TrangThai = TrangThaiGiaoHang.ChoGiao
                    }
                };

                foreach (var item in items)
                {
                    sachTheoMa[item.MaSach].SoLuongTon -= item.SoLuong;
                }

                _context.DonHangs.Add(don);
                await _context.SaveChangesAsync();
                await _context.Entry(don).Collection(x => x.ChiTietDonHangs).Query()
                    .Include(x => x.Sach)
                    .LoadAsync();
                _context.EmailOutbox.Add(
                    OrderEmailOutboxFactory.Confirmation(don, khachHang.TaiKhoan.Email));
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                _gioHang.XoaGio();
                TempData["success"] = "Đặt hàng thành công!";
                return RedirectToAction(nameof(ChiTiet), new { id = don.MaDH });
            }
            catch (DbUpdateException ex) when (LaTrungYeuCauDatHang(ex))
            {
                await tx.RollbackAsync();
                _context.ChangeTracker.Clear();
                var previousOrderId = await _context.DonHangs
                    .AsNoTracking()
                    .Where(x => x.MaYeuCau == model.MaYeuCau && x.MaKH == maKH.Value)
                    .Select(x => (int?)x.MaDH)
                    .FirstOrDefaultAsync();
                if (previousOrderId == null) throw;

                _gioHang.XoaGio();
                return RedirectToAction(nameof(ChiTiet), new { id = previousOrderId });
            }
            catch (Exception exception)
            {
                await tx.RollbackAsync();
                _logger.LogError(exception, "Failed to place order for customer {CustomerId}.", maKH);
                TempData["error"] = "Không thể đặt hàng. Vui lòng thử lại.";
                return RedirectToAction("Index", "GioHang");
            }
        }

        private async Task<(KhuyenMai? KhuyenMai, string? Loi, decimal GiamGia)> KiemTraKhuyenMaiAsync(
            string? maCode,
            decimal tongHang,
            bool batBuoc)
        {
            if (string.IsNullOrWhiteSpace(maCode))
            {
                return batBuoc
                    ? (null, "Vui lòng nhập mã khuyến mãi.", 0)
                    : (null, null, 0);
            }

            var maChuanHoa = KhuyenMaiHelper.ChuanHoaMa(maCode);
            var khuyenMai = await _context.KhuyenMais.SingleOrDefaultAsync(x => x.MaCode == maChuanHoa);
            if (khuyenMai == null)
            {
                return (null, "Mã khuyến mãi không hợp lệ.", 0);
            }

            var loi = KhuyenMaiHelper.KiemTraApDung(khuyenMai, tongHang, DateTime.UtcNow);
            return loi == null
                ? (khuyenMai, null, KhuyenMaiHelper.TinhSoTienGiam(khuyenMai, tongHang))
                : (null, loi, 0);
        }

        public async Task<IActionResult> LichSu()
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (maKH == null) return RedirectToAction("HoSo", "TaiKhoan");
            var list = await _context.DonHangs
                .Where(x => x.MaKH == maKH)
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> ChiTiet(int? id)
        {
            var don = await LayDonCuaToi(id);
            if (don == null) return NotFound();
            return View(don);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            var don = await LayDonCuaToi(id);
            if (don == null) return NotFound();
            if (don.TrangThai != TrangThaiDonHang.ChoXacNhan)
            {
                TempData["error"] = "Chỉ hủy được đơn đang chờ xác nhận.";
                return RedirectToAction(nameof(ChiTiet), new { id });
            }

            var previousStatus = don.TrangThai;
            don.TrangThai = TrangThaiDonHang.DaHuy;
            don.TrangThaiVersion++;
            foreach (var ct in don.ChiTietDonHangs)
            {
                if (ct.Sach != null)
                {
                    ct.Sach.SoLuongTon += ct.SoLuong;
                }
            }

            var recipient = don.KhachHang?.TaiKhoan?.Email;
            if (!string.IsNullOrWhiteSpace(recipient))
            {
                _context.EmailOutbox.Add(OrderEmailOutboxFactory.StatusChanged(
                    don, previousStatus, don.TrangThai, recipient));
            }
            else
            {
                _logger.LogWarning("No customer email is available for order {OrderId}.", don.MaDH);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Đã hủy đơn hàng.";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        private async Task<DonHang?> LayDonCuaToi(int? id)
        {
            var maKH = HttpContext.Session.GetInt32(SessionKeys.MaKH);
            if (id == null || maKH == null) return null;
            return await _context.DonHangs
                .Include(x => x.KhachHang).ThenInclude(x => x!.TaiKhoan)
                .Include(x => x.GiaoHang)
                .Include(x => x.ChiTietDonHangs).ThenInclude(x => x.Sach)
                .FirstOrDefaultAsync(x => x.MaDH == id && x.MaKH == maKH);
        }

        private async Task<(List<GioHangItem> Items, string? Error)> LayGioHangXacNhanAsync()
        {
            var gio = _gioHang.LayGio();
            if (gio.Count == 0) return (new List<GioHangItem>(), "Giỏ hàng trống.");
            if (gio.Any(x => x.MaSach <= 0 || x.SoLuong <= 0))
                return (new List<GioHangItem>(), "Thông tin giỏ hàng không hợp lệ. Vui lòng cập nhật lại giỏ hàng.");

            var soLuongTheoMa = gio
                .GroupBy(x => x.MaSach)
                .Select(g => new
                {
                    MaSach = g.Key,
                    SoLuong = g.Sum(x => (long)x.SoLuong)
                })
                .ToList();
            if (soLuongTheoMa.Any(x => x.SoLuong > int.MaxValue))
                return (new List<GioHangItem>(), "Số lượng sách trong giỏ hàng không hợp lệ.");

            var maSachs = soLuongTheoMa.Select(x => x.MaSach).ToArray();
            var sachTrongCSDL = await _context.Sachs
                .AsNoTracking()
                .Where(x => maSachs.Contains(x.MaSach))
                .ToDictionaryAsync(x => x.MaSach);
            if (sachTrongCSDL.Count != soLuongTheoMa.Count)
                return (new List<GioHangItem>(), "Một hoặc nhiều sách trong giỏ hàng không còn tồn tại.");

            var items = soLuongTheoMa.Select(x =>
            {
                var sach = sachTrongCSDL[x.MaSach];
                return new GioHangItem
                {
                    MaSach = sach.MaSach,
                    TenSach = sach.TenSach,
                    AnhBia = sach.AnhBia,
                    GiaBan = sach.GiaBan,
                    SoLuong = (int)x.SoLuong
                };
            }).ToList();

            _gioHang.LuuGio(items);
            return (items, null);
        }

        private static DatHangViewModel TaoDatHangViewModel(
            List<GioHangItem> items,
            List<DiaChi> diaChis,
            List<SoDienThoai> sdts,
            DatHangViewModel? input = null)
        {
            return new DatHangViewModel
            {
                MaYeuCau = input?.MaYeuCau ?? Guid.NewGuid(),
                MaDC = input?.MaDC ?? diaChis.FirstOrDefault(x => x.MacDinh)?.MaDC ?? diaChis[0].MaDC,
                MaSDT = input?.MaSDT ?? sdts.FirstOrDefault(x => x.MacDinh)?.MaSDT ?? sdts[0].MaSDT,
                PhuongThucThanhToan = input?.PhuongThucThanhToan ?? PhuongThucThanhToan.COD,
                GhiChu = input?.GhiChu,
                Items = items,
                TongHang = items.Sum(x => x.ThanhTien),
                GiamGia = 0,
                TongTien = items.Sum(x => x.ThanhTien),
                MaKhuyenMai = input?.MaKhuyenMai,
                DiaChis = new SelectList(diaChis, "MaDC", "DiaChiChiTiet", input?.MaDC),
                SoDienThoais = new SelectList(sdts, "MaSDT", "SoDT", input?.MaSDT)
            };
        }

        private static bool LaTrungYeuCauDatHang(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "IX_DonHangs_MaYeuCau"
            };
        }
    }
}
