using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanSach.Models
{
    public class Sach
    {
        [Key]
        public int MaSach { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        [StringLength(250)]
        [Display(Name = "Tên sách")]
        public string TenSach { get; set; } = string.Empty;

        [Display(Name = "Chủ đề")]
        public int MaCD { get; set; }

        [Display(Name = "Nhóm sách")]
        public int MaNhom { get; set; }

        [Display(Name = "Nhà xuất bản")]
        public int MaNXB { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá bán không hợp lệ")]
        [Display(Name = "Giá bán")]
        public decimal GiaBan { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ")]
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }

        [StringLength(300)]
        [Display(Name = "Ảnh bìa")]
        public string? AnhBia { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Năm xuất bản")]
        public int? NamXB { get; set; }

        [Display(Name = "Sách mới")]
        public bool SachMoi { get; set; }

        [Display(Name = "Nổi bật")]
        public bool NoiBat { get; set; }

        public ChuDe? ChuDe { get; set; }
        public NhomSach? NhomSach { get; set; }
        public NhaXuatBan? NhaXuatBan { get; set; }
        public ICollection<SachTacGia> SachTacGias { get; set; } = new List<SachTacGia>();
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}
