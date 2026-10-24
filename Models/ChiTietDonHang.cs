using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanSach.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public int MaCT { get; set; }

        public int MaDH { get; set; }
        public int MaSach { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        public DonHang? DonHang { get; set; }
        public Sach? Sach { get; set; }
    }
}
