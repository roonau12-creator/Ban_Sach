using System.ComponentModel.DataAnnotations;
using BanSach.Models;
using BanSach.Models.ViewModels;

namespace BanSach.Tests.Models
{
    public class KhachHangValidationTests
    {
        [Fact]
        public void SoDienThoai_KhongHopLe_BiTuChoi()
        {
            var model = new SoDienThoai { SoDT = "khong-phai-so" };

            var ketQua = Validator.TryValidateObject(
                model,
                new ValidationContext(model),
                new List<ValidationResult>(),
                validateAllProperties: true);

            Assert.False(ketQua);
        }

        [Fact]
        public void HoSo_EmailQuaDai_BiTuChoi()
        {
            var model = new HoSoViewModel
            {
                HoTen = "Nguyễn Văn A",
                Email = new string('a', 142) + "@mail.com"
            };

            var ketQua = Validator.TryValidateObject(
                model,
                new ValidationContext(model),
                new List<ValidationResult>(),
                validateAllProperties: true);

            Assert.False(ketQua);
        }
    }
}
