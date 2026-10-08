using BanSach.Helpers;

namespace BanSach.Tests.Helpers
{
    public class HienThiTests
    {
        [Theory]
        [InlineData(TrangThaiDonHang.ChoXacNhan, "Chờ xác nhận")]
        [InlineData(TrangThaiDonHang.DaXacNhan, "Đã xác nhận")]
        [InlineData(TrangThaiDonHang.DangGiao, "Đang giao")]
        [InlineData(TrangThaiDonHang.DaGiao, "Đã giao")]
        [InlineData(TrangThaiDonHang.DaHuy, "Đã hủy")]
        public void TrangThaiDon_GiaTriHopLe_TraVeNhanTiengViet(string value, string expected)
        {
            Assert.Equal(expected, HienThi.TrangThaiDon(value));
        }

        [Fact]
        public void TrangThaiDon_GiaTriLaRong_TraVeRong()
        {
            Assert.Equal("", HienThi.TrangThaiDon(""));
        }

        [Fact]
        public void TrangThaiDon_KhongNhap_TraVeRong()
        {
            Assert.Equal("", HienThi.TrangThaiDon(null));
        }

        [Fact]
        public void TrangThaiDon_GiaTriKhongBiet_TraVeNguyenGiaTri()
        {
            Assert.Equal("BatKy", HienThi.TrangThaiDon("BatKy"));
        }

        [Theory]
        [InlineData(TrangThaiDonHang.ChoXacNhan, "bg-warning text-dark")]
        [InlineData(TrangThaiDonHang.DaGiao, "bg-success")]
        [InlineData(TrangThaiDonHang.DaHuy, "bg-secondary")]
        public void BadgeTrangThaiDon_GiaTriHopLe_TraVeCssDung(string value, string expected)
        {
            Assert.Equal(expected, HienThi.BadgeTrangThaiDon(value));
        }

        [Fact]
        public void BadgeTrangThaiDon_KhongBiet_TraVeMacDinh()
        {
            Assert.Equal("bg-light text-dark", HienThi.BadgeTrangThaiDon("xyz"));
        }

        [Theory]
        [InlineData(TrangThaiThanhToan.DaThanhToan, "Đã thanh toán")]
        [InlineData(TrangThaiThanhToan.ChuaThanhToan, "Chưa thanh toán")]
        public void ThanhToan_GiaTriHopLe_TraVeNhan(string value, string expected)
        {
            Assert.Equal(expected, HienThi.ThanhToan(value));
        }

        [Theory]
        [InlineData(PhuongThucThanhToan.COD, "Thanh toán khi nhận hàng")]
        [InlineData(PhuongThucThanhToan.ChuyenKhoan, "Chuyển khoản")]
        public void PhuongThuc_GiaTriHopLe_TraVeNhan(string value, string expected)
        {
            Assert.Equal(expected, HienThi.PhuongThuc(value));
        }

        [Theory]
        [InlineData(TrangThaiGiaoHang.ChoGiao, "Chờ giao")]
        [InlineData(TrangThaiGiaoHang.DangGiao, "Đang giao")]
        [InlineData(TrangThaiGiaoHang.DaGiao, "Đã giao")]
        public void GiaoHang_GiaTriHopLe_TraVeNhan(string value, string expected)
        {
            Assert.Equal(expected, HienThi.GiaoHang(value));
        }

        [Fact]
        public void Tien_KhongDauPhanLap_DauDongTienDung()
        {
            var ketQua = HienThi.Tien(150_000m);

            Assert.Contains("150", ketQua);
            Assert.EndsWith("₫", ketQua);
        }

        [Fact]
        public void Tien_SoKhong_LaSoKhong()
        {
            Assert.StartsWith("0", HienThi.Tien(0m));
        }
    }
}
