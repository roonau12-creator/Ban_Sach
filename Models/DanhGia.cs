using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class DanhGia
    {
        [Key]
        public int MaDG { get; set; }

        public int MaTK { get; set; }
        public int MaSach { get; set; }

        [Range(1, 5)]
        [Display(Name = "Số sao")]
        public int SoSao { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 5)]
        [Display(Name = "Nhận xét")]
        public string NoiDung { get; set; } = string.Empty;

        [Display(Name = "Ngày đánh giá")]
        public DateTime NgayDanhGia { get; set; } = DateTime.UtcNow;

        [Display(Name = "Đã ẩn")]
        public bool An { get; set; }

        public TaiKhoan? TaiKhoan { get; set; }
        public Sach? Sach { get; set; }
    }
}
