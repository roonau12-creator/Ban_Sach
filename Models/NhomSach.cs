using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class NhomSach
    {
        [Key]
        public int MaNhom { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nhóm sách")]
        [StringLength(100)]
        [Display(Name = "Tên nhóm")]
        public string TenNhom { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true;

        [Display(Name = "Thứ tự")]
        public int ThuTu { get; set; }

        public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
    }
}
