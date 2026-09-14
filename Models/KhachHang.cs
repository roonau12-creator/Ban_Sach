using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKH { get; set; }

        public int MaTK { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        public TaiKhoan? TaiKhoan { get; set; }
        public ICollection<DiaChi> DiaChis { get; set; } = new List<DiaChi>();
        public ICollection<SoDienThoai> SoDienThoais { get; set; } = new List<SoDienThoai>();
        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}
