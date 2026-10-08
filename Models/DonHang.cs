using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BanSach.Helpers;

namespace BanSach.Models
{
    public class DonHang
    {
        [Key]
        public int MaDH { get; set; }

        public int MaKH { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime NgayDat { get; set; } = DateTime.UtcNow;

        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiDonHang.ChoXacNhan;

        [Column(TypeName = "numeric(18,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }

        [StringLength(30)]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = Helpers.PhuongThucThanhToan.COD;

        [StringLength(30)]
        [Display(Name = "Thanh toán")]
        public string TrangThaiThanhToan { get; set; } = Helpers.TrangThaiThanhToan.ChuaThanhToan;

        public int? MaDC { get; set; }
        public int? MaSDT { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Địa chỉ giao")]
        public string DiaChiGiao { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoaiGiao { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public KhachHang? KhachHang { get; set; }
        public DiaChi? DiaChi { get; set; }
        public SoDienThoai? SoDienThoai { get; set; }
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
        public GiaoHang? GiaoHang { get; set; }
    }
}
