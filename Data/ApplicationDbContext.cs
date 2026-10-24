using BanSach.Models;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ChuDe> ChuDes { get; set; }
        public DbSet<NhomSach> NhomSachs { get; set; }
        public DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public DbSet<TacGia> TacGias { get; set; }
        public DbSet<Sach> Sachs { get; set; }
        public DbSet<SachTacGia> SachTacGias { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DiaChi> DiaChis { get; set; }
        public DbSet<SoDienThoai> SoDienThoais { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<GiaoHang> GiaoHangs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(x => x.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<SachTacGia>()
                .HasKey(x => new { x.MaSach, x.MaTG });

            modelBuilder.Entity<SachTacGia>()
                .HasOne(x => x.Sach)
                .WithMany(x => x.SachTacGias)
                .HasForeignKey(x => x.MaSach)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SachTacGia>()
                .HasOne(x => x.TacGia)
                .WithMany(x => x.SachTacGias)
                .HasForeignKey(x => x.MaTG)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sach>()
                .HasOne(x => x.ChuDe)
                .WithMany(x => x.Sachs)
                .HasForeignKey(x => x.MaCD)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sach>()
                .HasOne(x => x.NhomSach)
                .WithMany(x => x.Sachs)
                .HasForeignKey(x => x.MaNhom)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sach>()
                .HasOne(x => x.NhaXuatBan)
                .WithMany(x => x.Sachs)
                .HasForeignKey(x => x.MaNXB)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KhachHang>()
                .HasOne(x => x.TaiKhoan)
                .WithOne(x => x.KhachHang)
                .HasForeignKey<KhachHang>(x => x.MaTK)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiaChi>()
                .HasOne(x => x.KhachHang)
                .WithMany(x => x.DiaChis)
                .HasForeignKey(x => x.MaKH)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SoDienThoai>()
                .HasOne(x => x.KhachHang)
                .WithMany(x => x.SoDienThoais)
                .HasForeignKey(x => x.MaKH)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DonHang>()
                .HasOne(x => x.KhachHang)
                .WithMany(x => x.DonHangs)
                .HasForeignKey(x => x.MaKH)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DonHang>()
                .HasOne(x => x.DiaChi)
                .WithMany()
                .HasForeignKey(x => x.MaDC)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DonHang>()
                .HasOne(x => x.SoDienThoai)
                .WithMany()
                .HasForeignKey(x => x.MaSDT)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(x => x.DonHang)
                .WithMany(x => x.ChiTietDonHangs)
                .HasForeignKey(x => x.MaDH)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChiTietDonHang>()
                .HasOne(x => x.Sach)
                .WithMany(x => x.ChiTietDonHangs)
                .HasForeignKey(x => x.MaSach)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GiaoHang>()
                .HasOne(x => x.DonHang)
                .WithOne(x => x.GiaoHang)
                .HasForeignKey<GiaoHang>(x => x.MaDH)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
