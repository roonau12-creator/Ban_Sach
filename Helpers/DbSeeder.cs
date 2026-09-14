using BanSach.Data;
using BanSach.Models;
using Microsoft.AspNetCore.Identity;

namespace BanSach.Helpers
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.TaiKhoans.Any(x => x.VaiTro == VaiTroNguoiDung.Admin))
            {
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = "admin",
                    Email = "admin@bansach.local",
                    VaiTro = VaiTroNguoiDung.Admin,
                    TrangThai = true
                };

                var hasher = new PasswordHasher<TaiKhoan>();
                taiKhoan.MatKhauHash = hasher.HashPassword(taiKhoan, "Admin@123");

                context.TaiKhoans.Add(taiKhoan);
                context.SaveChanges();
            }

            if (!context.ChuDes.Any())
            {
                context.ChuDes.AddRange(
                    new ChuDe { TenChuDe = "Văn học" },
                    new ChuDe { TenChuDe = "Kinh tế" },
                    new ChuDe { TenChuDe = "Kỹ năng sống" }
                );
                context.SaveChanges();
            }

            if (!context.NhomSachs.Any())
            {
                context.NhomSachs.AddRange(
                    new NhomSach { TenNhom = "Bán chạy", TrangThai = true, ThuTu = 1 },
                    new NhomSach { TenNhom = "Thiếu nhi", TrangThai = true, ThuTu = 2 }
                );
                context.SaveChanges();
            }

            if (!context.NhaXuatBans.Any())
            {
                context.NhaXuatBans.Add(new NhaXuatBan { TenNXB = "NXB Trẻ", DiaChi = "TP.HCM", DienThoai = "02839316289" });
                context.SaveChanges();
            }

            if (!context.TacGias.Any())
            {
                context.TacGias.AddRange(
                    new TacGia { TenTacGia = "Nguyễn Nhật Ánh", TieuSu = "Nhà văn Việt Nam nổi tiếng với tác phẩm tuổi học trò." },
                    new TacGia { TenTacGia = "Dale Carnegie", TieuSu = "Tác giả sách kỹ năng sống." }
                );
                context.SaveChanges();
            }

            if (!context.Sachs.Any())
            {
                var cd = context.ChuDes.First();
                var nhom = context.NhomSachs.First();
                var nxb = context.NhaXuatBans.First();
                var tg1 = context.TacGias.First();
                var sach = new Sach
                {
                    TenSach = "Tôi thấy hoa vàng trên cỏ xanh",
                    MaCD = cd.MaCD,
                    MaNhom = nhom.MaNhom,
                    MaNXB = nxb.MaNXB,
                    GiaBan = 98000,
                    SoLuongTon = 20,
                    MoTa = "Truyện dài tuổi thơ đầy cảm xúc.",
                    NamXB = 2010,
                    SachMoi = true,
                    NoiBat = true
                };
                context.Sachs.Add(sach);
                context.SaveChanges();
                context.SachTacGias.Add(new SachTacGia { MaSach = sach.MaSach, MaTG = tg1.MaTG });
                context.SaveChanges();
            }
        }
    }
}
