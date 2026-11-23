using BanSach.Data;
using BanSach.Models;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Helpers
{
    public class GioHangService
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionGioHang _gioHang;

        public GioHangService(ApplicationDbContext context, SessionGioHang gioHang)
        {
            _context = context;
            _gioHang = gioHang;
        }

        public async Task<bool> DongBoAsync()
        {
            var items = _gioHang.LayGio();
            if (items.Count == 0) return false;

            var maSachs = items.Select(x => x.MaSach).Distinct().ToList();
            var sachs = await _context.Sachs
                .AsNoTracking()
                .Include(x => x.NhomSach)
                .Where(x => maSachs.Contains(x.MaSach))
                .ToDictionaryAsync(x => x.MaSach);
            var changed = false;

            foreach (var item in items.ToList())
            {
                if (!sachs.TryGetValue(item.MaSach, out var sach) || !CoTheBan(item, sach))
                {
                    items.Remove(item);
                    changed = true;
                    continue;
                }

                changed |= item.SoLuong != Math.Min(item.SoLuong, sach.SoLuongTon);
                item.SoLuong = Math.Min(item.SoLuong, sach.SoLuongTon);
                changed |= item.TenSach != sach.TenSach
                    || item.AnhBia != sach.AnhBia
                    || item.GiaBan != sach.GiaBan;
                item.TenSach = sach.TenSach;
                item.AnhBia = sach.AnhBia;
                item.GiaBan = sach.GiaBan;
            }

            if (changed) _gioHang.LuuGio(items);
            return changed;
        }

        public static bool CoTheBan(GioHangItem item, Sach sach)
        {
            return item.SoLuong > 0
                && sach.SoLuongTon > 0
                && sach.GiaBan >= 0
                && sach.NhomSach?.TrangThai == true;
        }
    }
}
