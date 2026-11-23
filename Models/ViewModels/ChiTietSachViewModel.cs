using System.ComponentModel.DataAnnotations;

namespace BanSach.Models.ViewModels
{
    public class ChiTietSachViewModel
    {
        public Sach Sach { get; set; } = null!;
        public List<DanhGia> DanhGias { get; set; } = new();
        public double DiemTrungBinh { get; set; }
        public bool DaDangNhap { get; set; }
        public bool CoTheDanhGia { get; set; }
        public bool DaDanhGia { get; set; }
        public DanhGiaFormViewModel DanhGiaMoi { get; set; } = new();
    }

    public class DanhGiaFormViewModel
    {
        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5.")]
        [Display(Name = "Số sao")]
        public int SoSao { get; set; } = 5;

        [Required(ErrorMessage = "Vui lòng nhập nhận xét.")]
        [StringLength(2000, MinimumLength = 5, ErrorMessage = "Nhận xét phải có từ 5 đến 2000 ký tự.")]
        [Display(Name = "Nhận xét")]
        public string NoiDung { get; set; } = string.Empty;
    }
}
