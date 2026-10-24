using BanSach.Models;

namespace BanSach.Tests.Models
{
    public class GioHangItemTests
    {
        [Fact]
        public void ThanhTien_TinhDungGiaNhanSoLuong()
        {
            var item = new GioHangItem { GiaBan = 100_000m, SoLuong = 2 };

            Assert.Equal(200_000m, item.ThanhTien);
        }

        [Fact]
        public void ThanhTien_SoLuongZero_LaKhong()
        {
            var item = new GioHangItem { GiaBan = 50_000m, SoLuong = 0 };

            Assert.Equal(0m, item.ThanhTien);
        }

        [Fact]
        public void ThanhTien_GiaAm_TinhDungDauAm()
        {
            var item = new GioHangItem { GiaBan = -10_000m, SoLuong = 3 };

            Assert.Equal(-30_000m, item.ThanhTien);
        }

        [Fact]
        public void TenSach_MacDinh_LaRong()
        {
            var item = new GioHangItem();

            Assert.Equal(string.Empty, item.TenSach);
        }
    }
}
