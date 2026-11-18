using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class ThongBao
    {
        [Key]
        public int MaTB { get; set; }

        public int MaTK { get; set; }

        public int? MaDH { get; set; }

        [Required]
        [StringLength(200)]
        public string MaSuKien { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string NoiDung { get; set; } = string.Empty;

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        public bool DaDoc { get; set; }

        public bool An { get; set; }

        public TaiKhoan? TaiKhoan { get; set; }

        public DonHang? DonHang { get; set; }
    }
}
