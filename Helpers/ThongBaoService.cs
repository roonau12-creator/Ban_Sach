using BanSach.Data;
using BanSach.Models;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Helpers
{
    public class ThongBaoService
    {
        private readonly ApplicationDbContext _context;

        public ThongBaoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task TaoChoKhachHangAsync(
            DonHang donHang,
            string maSuKien,
            string tieuDe,
            string noiDung)
        {
            var maTK = await _context.KhachHangs
                .Where(x => x.MaKH == donHang.MaKH)
                .Select(x => x.MaTK)
                .SingleAsync();

            await ThemNeuChuaCoAsync(maTK, donHang.MaDH, maSuKien, tieuDe, noiDung);
        }

        public async Task TaoChoQuanTriVienAsync(
            DonHang donHang,
            string maSuKien,
            string tieuDe,
            string noiDung)
        {
            var maTks = await _context.TaiKhoans
                .Where(x => x.VaiTro == VaiTroNguoiDung.Admin && x.TrangThai)
                .Select(x => x.MaTK)
                .ToListAsync();

            foreach (var maTK in maTks)
            {
                await ThemNeuChuaCoAsync(maTK, donHang.MaDH, maSuKien, tieuDe, noiDung);
            }
        }

        private async Task ThemNeuChuaCoAsync(
            int maTK,
            int? maDH,
            string maSuKien,
            string tieuDe,
            string noiDung)
        {
            if (_context.ThongBaos.Local.Any(x => x.MaTK == maTK && x.MaSuKien == maSuKien)
                || await _context.ThongBaos.AnyAsync(x => x.MaTK == maTK && x.MaSuKien == maSuKien))
            {
                return;
            }

            _context.ThongBaos.Add(new ThongBao
            {
                MaTK = maTK,
                MaDH = maDH,
                MaSuKien = maSuKien,
                TieuDe = tieuDe,
                NoiDung = noiDung,
                NgayTao = DateTime.UtcNow
            });
        }
    }
}
