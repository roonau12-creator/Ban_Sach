using System.ComponentModel.DataAnnotations;
using BanSach.Helpers;
using BanSach.Models;

namespace BanSach.Tests.Helpers
{
    public class KhuyenMaiHelperTests
    {
        private static readonly DateTime ThoiDiemUtc = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void TinhSoTienGiam_ApDungPhanTramVaGioiHanToiDa()
        {
            var khuyenMai = TaoKhuyenMai(LoaiGiamGiaKhuyenMai.PhanTram, 20, giamToiDa: 150);

            Assert.Equal(150, KhuyenMaiHelper.TinhSoTienGiam(khuyenMai, 1000));
        }

        [Fact]
        public void TinhSoTienGiam_KhongVuotQuaGiaTriDonHang()
        {
            var khuyenMai = TaoKhuyenMai(LoaiGiamGiaKhuyenMai.SoTien, 500);

            Assert.Equal(100, KhuyenMaiHelper.TinhSoTienGiam(khuyenMai, 100));
        }

        [Fact]
        public void KiemTraApDung_TuChoiMaKhongHoatDong()
        {
            var khuyenMai = TaoKhuyenMai(LoaiGiamGiaKhuyenMai.PhanTram, 10);
            khuyenMai.DangHoatDong = false;

            Assert.NotNull(KhuyenMaiHelper.KiemTraApDung(khuyenMai, 1000, ThoiDiemUtc));
        }

        [Fact]
        public void KiemTraApDung_TuChoiMaNgoaiHanSuDung()
        {
            var khuyenMai = TaoKhuyenMai(LoaiGiamGiaKhuyenMai.PhanTram, 10);

            Assert.NotNull(KhuyenMaiHelper.KiemTraApDung(khuyenMai, 1000, ThoiDiemUtc.AddDays(-2)));
            Assert.NotNull(KhuyenMaiHelper.KiemTraApDung(khuyenMai, 1000, ThoiDiemUtc.AddDays(2)));
        }

        [Fact]
        public void KiemTraApDung_TuChoiDonHangDuoiMucToiThieu()
        {
            var khuyenMai = TaoKhuyenMai(LoaiGiamGiaKhuyenMai.PhanTram, 10, toiThieu: 500);

            Assert.NotNull(KhuyenMaiHelper.KiemTraApDung(khuyenMai, 499, ThoiDiemUtc));
            Assert.Null(KhuyenMaiHelper.KiemTraApDung(khuyenMai, 500, ThoiDiemUtc));
        }

        [Fact]
        public void ChuanHoaMa_XoaKhoangTrangVaChuyenThanhChuHoa()
        {
            Assert.Equal("SALE2026", KhuyenMaiHelper.ChuanHoaMa(" sale2026 "));
        }

        [Fact]
        public void KhuyenMai_TuChoiTyLeGiamVuotQuaMotTramPhanTram()
        {
            var khuyenMai = TaoKhuyenMai(LoaiGiamGiaKhuyenMai.PhanTram, 101);

            var errors = new List<ValidationResult>();
            var hopLe = Validator.TryValidateObject(
                khuyenMai,
                new ValidationContext(khuyenMai),
                errors,
                validateAllProperties: true);

            Assert.False(hopLe);
            Assert.Contains(errors, x => x.MemberNames.Contains(nameof(KhuyenMai.GiaTriGiam)));
        }

        [Fact]
        public void KhuyenMai_TuChoiThoiGianKetThucKhongSauThoiGianBatDau()
        {
            var khuyenMai = TaoKhuyenMai(LoaiGiamGiaKhuyenMai.SoTien, 100);
            khuyenMai.NgayKetThuc = khuyenMai.NgayBatDau;

            var errors = new List<ValidationResult>();
            var hopLe = Validator.TryValidateObject(
                khuyenMai,
                new ValidationContext(khuyenMai),
                errors,
                validateAllProperties: true);

            Assert.False(hopLe);
            Assert.Contains(errors, x => x.MemberNames.Contains(nameof(KhuyenMai.NgayKetThuc)));
        }

        private static KhuyenMai TaoKhuyenMai(
            string loai,
            decimal giaTri,
            decimal? giamToiDa = null,
            decimal toiThieu = 0)
        {
            return new KhuyenMai
            {
                MaCode = "SALE",
                TenKhuyenMai = "Giảm giá",
                LoaiGiamGia = loai,
                GiaTriGiam = giaTri,
                GiamToiDa = giamToiDa,
                DonHangToiThieu = toiThieu,
                NgayBatDau = ThoiDiemUtc.AddDays(-1),
                NgayKetThuc = ThoiDiemUtc.AddDays(1)
            };
        }
    }
}
