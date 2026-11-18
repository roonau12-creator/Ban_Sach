using BanSach.Models;
using BanSach.Models.ViewModels;

namespace BanSach.Helpers
{
    public static class SachBanChayQuery
    {
        public static IQueryable<SachBanChayItem> LayTopSachBanChay(IQueryable<ChiTietDonHang> chiTietDonHangs)
        {
            return chiTietDonHangs
                .Where(x => x.DonHang != null && x.DonHang.TrangThai != TrangThaiDonHang.DaHuy)
                .GroupBy(x => new
                {
                    x.MaSach,
                    x.Sach!.TenSach,
                    x.Sach!.AnhBia,
                    x.Sach!.GiaBan
                })
                .Select(g => new SachBanChayItem
                {
                    MaSach = g.Key.MaSach,
                    TenSach = g.Key.TenSach,
                    AnhBia = g.Key.AnhBia,
                    GiaBan = g.Key.GiaBan,
                    SoLuongBan = g.Sum(x => x.SoLuong)
                })
                .OrderByDescending(x => x.SoLuongBan)
                .ThenBy(x => x.MaSach)
                .Take(8);
        }
    }
}
