using BanSach.Models;

namespace BanSach.Helpers
{
    public static class KhuyenMaiHelper
    {
        public static string ChuanHoaMa(string? maCode) => (maCode ?? string.Empty).Trim().ToUpperInvariant();

        public static string? KiemTraApDung(KhuyenMai khuyenMai, decimal tongHang, DateTime thoiDiemUtc)
        {
            if (!khuyenMai.DangHoatDong)
            {
                return "Mã khuyến mãi hiện không hoạt động.";
            }

            if (thoiDiemUtc < khuyenMai.NgayBatDau || thoiDiemUtc > khuyenMai.NgayKetThuc)
            {
                return "Mã khuyến mãi chưa bắt đầu hoặc đã hết hạn.";
            }

            if (tongHang < khuyenMai.DonHangToiThieu)
            {
                return $"Đơn hàng chưa đạt giá trị tối thiểu {HienThi.Tien(khuyenMai.DonHangToiThieu)}.";
            }

            return null;
        }

        public static decimal TinhSoTienGiam(KhuyenMai khuyenMai, decimal tongHang)
        {
            var giamGia = khuyenMai.LoaiGiamGia == LoaiGiamGiaKhuyenMai.PhanTram
                ? Math.Round(tongHang * khuyenMai.GiaTriGiam / 100, 2, MidpointRounding.AwayFromZero)
                : khuyenMai.GiaTriGiam;

            if (khuyenMai.GiamToiDa.HasValue)
            {
                giamGia = Math.Min(giamGia, khuyenMai.GiamToiDa.Value);
            }

            return Math.Min(Math.Max(giamGia, 0), Math.Max(tongHang, 0));
        }
    }
}
