using System.ComponentModel.DataAnnotations;
using BanSach.Models;
using BanSach.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace BanSach.Tests.Models
{
    public class DangKyViewModelTests
    {
        [Fact]
        public void DangKy_DuLieuHopLe_KhongCoLoiValidation()
        {
            var model = TaoModelHopLe();

            Assert.Empty(LayLoiValidation(model));
        }

        [Fact]
        public void DangKy_EmailKhongHopLe_TraVeLoiValidation()
        {
            var model = TaoModelHopLe();
            model.Email = "email-khong-hop-le";

            Assert.Contains(LayLoiValidation(model), loi => loi.MemberNames.Contains(nameof(model.Email)));
        }

        [Fact]
        public void DangKy_EmailVuotQuaDoDaiChoPhep_TraVeLoiValidation()
        {
            var model = TaoModelHopLe();
            model.Email = $"{new string('a', 140)}@example.com";

            Assert.Contains(LayLoiValidation(model), loi => loi.MemberNames.Contains(nameof(model.Email)));
        }

        [Fact]
        public void DangKy_MatKhauXacNhanKhongKhop_TraVeLoiValidation()
        {
            var model = TaoModelHopLe();
            model.XacNhanMatKhau = "MatKhauKhac";

            Assert.Contains(LayLoiValidation(model), loi =>
                loi.MemberNames.Contains(nameof(model.XacNhanMatKhau)));
        }

        [Fact]
        public void MatKhau_HasherMaHoaVaTuChoiMatKhauSai()
        {
            var taiKhoan = new TaiKhoan();
            const string matKhau = "MatKhauAnToan123";
            var hasher = new PasswordHasher<TaiKhoan>();

            var hash = hasher.HashPassword(taiKhoan, matKhau);

            Assert.NotEqual(matKhau, hash);
            Assert.Equal(PasswordVerificationResult.Success, hasher.VerifyHashedPassword(taiKhoan, hash, matKhau));
            Assert.Equal(PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(taiKhoan, hash, "MatKhauSai"));
        }

        private static DangKyViewModel TaoModelHopLe() => new()
        {
            HoTen = "Nguyen Van A",
            TenDangNhap = "nguyenvana",
            Email = "a@example.com",
            MatKhau = "MatKhau123",
            XacNhanMatKhau = "MatKhau123"
        };

        private static List<ValidationResult> LayLoiValidation(DangKyViewModel model)
        {
            var ketQua = new List<ValidationResult>();
            Validator.TryValidateObject(model, new ValidationContext(model), ketQua, validateAllProperties: true);
            return ketQua;
        }
    }
}
