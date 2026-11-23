using BanSach.Helpers;
using BanSach.Models;

namespace BanSach.Tests.Helpers
{
    public class GioHangServiceTests
    {
        [Fact]
        public void CoTheBan_SachConBan_CoTonKho_TraVeTrue()
        {
            var item = new GioHangItem { SoLuong = 2 };
            var sach = new Sach
            {
                SoLuongTon = 5,
                NhomSach = new NhomSach { TrangThai = true }
            };

            Assert.True(GioHangService.CoTheBan(item, sach));
        }

        [Fact]
        public void CoTheBan_SachHetHang_TraVeFalse()
        {
            var item = new GioHangItem { SoLuong = 1 };
            var sach = new Sach
            {
                SoLuongTon = 0,
                NhomSach = new NhomSach { TrangThai = true }
            };

            Assert.False(GioHangService.CoTheBan(item, sach));
        }

        [Fact]
        public void CoTheBan_NhomSachNgungKinhDoanh_TraVeFalse()
        {
            var item = new GioHangItem { SoLuong = 1 };
            var sach = new Sach
            {
                SoLuongTon = 5,
                NhomSach = new NhomSach { TrangThai = false }
            };

            Assert.False(GioHangService.CoTheBan(item, sach));
        }

        [Fact]
        public void CoTheBan_SoLuongKhongHopLe_TraVeFalse()
        {
            var item = new GioHangItem { SoLuong = 0 };
            var sach = new Sach
            {
                SoLuongTon = 5,
                NhomSach = new NhomSach { TrangThai = true }
            };

            Assert.False(GioHangService.CoTheBan(item, sach));
        }

        [Fact]
        public void CoTheBan_GiaAm_TraVeFalse()
        {
            var item = new GioHangItem { SoLuong = 1 };
            var sach = new Sach
            {
                GiaBan = -1m,
                SoLuongTon = 5,
                NhomSach = new NhomSach { TrangThai = true }
            };

            Assert.False(GioHangService.CoTheBan(item, sach));
        }
    }
}
