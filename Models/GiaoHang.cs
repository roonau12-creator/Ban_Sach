using System.ComponentModel.DataAnnotations;
using BanSach.Helpers;

namespace BanSach.Models
{
    public class GiaoHang
    {
        [Key]
        public int MaGH { get; set; }

        public int MaDH { get; set; }

        [StringLength(30)]
        [Display(Name = "Trạng thái giao")]
        public string TrangThai { get; set; } = TrangThaiGiaoHang.ChoGiao;

        [Display(Name = "Ngày giao")]
        public DateTime? NgayGiao { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public DonHang? DonHang { get; set; }
    }
}
