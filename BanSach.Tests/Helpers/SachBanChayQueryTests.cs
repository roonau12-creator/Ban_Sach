using BanSach.Helpers;
using BanSach.Models;

namespace BanSach.Tests.Helpers
{
    public class SachBanChayQueryTests
    {
        [Fact]
        public void LayTopSachBanChay_LoaiDonDaHuyVaTongHopTheoSoLuong()
        {
            var chiTietDonHangs = new[]
            {
                ChiTiet(10, "Sách A", 150_000m, 2, TrangThaiDonHang.DaGiao),
                ChiTiet(10, "Sách A", 150_000m, 4, TrangThaiDonHang.DaGiao),
                ChiTiet(20, "Sách B", 200_000m, 5, TrangThaiDonHang.DaGiao),
                ChiTiet(20, "Sách B", 200_000m, 2, TrangThaiDonHang.DangGiao),
                ChiTiet(30, "Sách C", 90_000m, 100, TrangThaiDonHang.DaHuy),
                ChiTiet(40, "Sách D", 80_000m, 50, TrangThaiDonHang.DangGiao),
                new ChiTietDonHang { MaSach = 50, SoLuong = 100 }
            }.AsQueryable();

            var result = SachBanChayQuery.LayTopSachBanChay(chiTietDonHangs).ToList();

            Assert.Equal(new[] { 40, 20, 10 }, result.Select(x => x.MaSach));
            Assert.Equal(new[] { 50, 7, 6 }, result.Select(x => x.SoLuongBan));
            Assert.Equal("Sách B", result[1].TenSach);
            Assert.Equal(200_000m, result[1].GiaBan);
        }

        private static ChiTietDonHang ChiTiet(
            int maSach,
            string tenSach,
            decimal giaBan,
            int soLuong,
            string trangThai)
        {
            return new ChiTietDonHang
            {
                MaSach = maSach,
                SoLuong = soLuong,
                Sach = new Sach
                {
                    MaSach = maSach,
                    TenSach = tenSach,
                    GiaBan = giaBan
                },
                DonHang = new DonHang
                {
                    TrangThai = trangThai
                }
            };
        }
    }
}
