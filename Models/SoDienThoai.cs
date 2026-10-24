using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class SoDienThoai
    {
        [Key]
        public int MaSDT { get; set; }

        public int MaKH { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string SoDT { get; set; } = string.Empty;

        [Display(Name = "Mặc định")]
        public bool MacDinh { get; set; }

        public KhachHang? KhachHang { get; set; }
    }
}
