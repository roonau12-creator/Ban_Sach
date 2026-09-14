using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class NhaXuatBan
    {
        [Key]
        public int MaNXB { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nhà xuất bản")]
        [StringLength(200)]
        [Display(Name = "Tên nhà xuất bản")]
        public string TenNXB { get; set; } = string.Empty;

        [StringLength(300)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(20)]
        [Display(Name = "Điện thoại")]
        public string? DienThoai { get; set; }

        public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
    }
}
