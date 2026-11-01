using BanSach.Helpers;
using BanSach.Models;

namespace BanSach.Tests.Models
{
    public class ThanhToanTests
    {
        [Fact]
        public void CapNhatTrangThai_DaThanhToan_LuuMaGiaoDichVaThoiGian()
        {
            var payment = TaoThanhToan(PhuongThucThanhToan.ChuyenKhoan);
            var thoiGian = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);

            payment.CapNhatTrangThai(TrangThaiThanhToan.DaThanhToan, 250_000m, thoiGian, "  TX-001  ");

            Assert.Equal(TrangThaiThanhToan.DaThanhToan, payment.TrangThai);
            Assert.Equal("TX-001", payment.MaGiaoDich);
            Assert.Equal(thoiGian, payment.NgayThanhToan);
            Assert.Equal(thoiGian, payment.NgayCapNhat);
        }

        [Fact]
        public void CapNhatTrangThai_SoTienKhongKhop_TuChoiXacNhan()
        {
            var payment = TaoThanhToan(PhuongThucThanhToan.COD);

            Assert.Throws<InvalidOperationException>(() =>
                payment.CapNhatTrangThai(TrangThaiThanhToan.DaThanhToan, 250_001m, DateTime.UtcNow, null));
            Assert.Equal(TrangThaiThanhToan.ChuaThanhToan, payment.TrangThai);
        }

        [Fact]
        public void CapNhatTrangThai_ChuyenKhoanKhongCoMaGiaoDich_TuChoiXacNhan()
        {
            var payment = TaoThanhToan(PhuongThucThanhToan.ChuyenKhoan);

            Assert.Throws<InvalidOperationException>(() =>
                payment.CapNhatTrangThai(TrangThaiThanhToan.DaThanhToan, 250_000m, DateTime.UtcNow, null));
        }

        [Fact]
        public void CapNhatTrangThai_GiaoDichDaThanhToanLapLai_KhongDoiThoiGian()
        {
            var payment = TaoThanhToan(PhuongThucThanhToan.ChuyenKhoan);
            var thoiGian = DateTime.UtcNow;
            payment.CapNhatTrangThai(TrangThaiThanhToan.DaThanhToan, 250_000m, thoiGian, "TX-001");

            payment.CapNhatTrangThai(TrangThaiThanhToan.DaThanhToan, 250_000m, thoiGian.AddMinutes(1), null);

            Assert.Equal(thoiGian, payment.NgayThanhToan);
            Assert.Equal(thoiGian, payment.NgayCapNhat);
            Assert.Equal("TX-001", payment.MaGiaoDich);
        }

        [Fact]
        public void CapNhatTrangThai_ThatBai_GhiNhanThoiGianCapNhat()
        {
            var payment = TaoThanhToan(PhuongThucThanhToan.ChuyenKhoan);
            var thoiGian = DateTime.UtcNow;

            payment.CapNhatTrangThai(TrangThaiThanhToan.ThatBai, 250_000m, thoiGian, "TX-FAILED");

            Assert.Equal(TrangThaiThanhToan.ThatBai, payment.TrangThai);
            Assert.Equal(thoiGian, payment.NgayCapNhat);
            Assert.Null(payment.NgayThanhToan);
        }

        [Fact]
        public void CapNhatTrangThai_KhongChoPhepGiamTrangThaiDaThanhToan()
        {
            var payment = TaoThanhToan(PhuongThucThanhToan.COD);
            payment.CapNhatTrangThai(TrangThaiThanhToan.DaThanhToan, 250_000m, DateTime.UtcNow, null);

            Assert.Throws<InvalidOperationException>(() =>
                payment.CapNhatTrangThai(TrangThaiThanhToan.ThatBai, 250_000m, DateTime.UtcNow, null));
        }

        private static ThanhToan TaoThanhToan(string phuongThuc) => new()
        {
            SoTien = 250_000m,
            PhuongThuc = phuongThuc,
            TrangThai = TrangThaiThanhToan.ChuaThanhToan
        };
    }
}
