using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class DiaChi
    {
        [Key]
        public int MaDC { get; set; }

        public int MaKH { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(300)]
        [Display(Name = "Địa chỉ")]
        public string DiaChiChiTiet { get; set; } = string.Empty;

        [Display(Name = "Mặc định")]
        public bool MacDinh { get; set; }

        public KhachHang? KhachHang { get; set; }
    }
}
