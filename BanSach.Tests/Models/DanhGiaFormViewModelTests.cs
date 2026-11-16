using System.ComponentModel.DataAnnotations;
using BanSach.Models.ViewModels;

namespace BanSach.Tests.Models
{
    public class DanhGiaFormViewModelTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        public void SoSao_TaiBien1Va5_HopLe(int soSao)
        {
            var results = Validate(new DanhGiaFormViewModel { SoSao = soSao, NoiDung = "Nội dung hợp lệ" });

            Assert.Empty(results);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void SoSao_NgoaiKhoang1Den5_KhongHopLe(int soSao)
        {
            var results = Validate(new DanhGiaFormViewModel { SoSao = soSao, NoiDung = "Nội dung hợp lệ" });

            Assert.Contains(results, x => x.MemberNames.Contains(nameof(DanhGiaFormViewModel.SoSao)));
        }

        [Theory]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData("bốn")]
        public void NoiDung_TrongHoacQuaNgan_KhongHopLe(string noiDung)
        {
            var results = Validate(new DanhGiaFormViewModel { SoSao = 5, NoiDung = noiDung });

            Assert.Contains(results, x => x.MemberNames.Contains(nameof(DanhGiaFormViewModel.NoiDung)));
        }

        [Fact]
        public void NoiDung_DuDoDai_HopLe()
        {
            var results = Validate(new DanhGiaFormViewModel { SoSao = 5, NoiDung = new string('a', 2000) });

            Assert.Empty(results);
        }

        [Fact]
        public void NoiDung_DaiHon2000KyTu_KhongHopLe()
        {
            var results = Validate(new DanhGiaFormViewModel { SoSao = 5, NoiDung = new string('a', 2001) });

            Assert.Contains(results, x => x.MemberNames.Contains(nameof(DanhGiaFormViewModel.NoiDung)));
        }

        private static IList<ValidationResult> Validate(DanhGiaFormViewModel model)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
            return results;
        }
    }
}
