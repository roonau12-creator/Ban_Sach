using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTK { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string MatKhauHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "Customer";

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        [StringLength(100)]
        public string? ResetToken { get; set; }

        public DateTime? ResetTokenHetHan { get; set; }

        public KhachHang? KhachHang { get; set; }
    }
}
