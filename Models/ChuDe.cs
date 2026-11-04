using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BanSach.Models
{
    [Index(nameof(TenChuDeNormalized), IsUnique = true)]
    public class ChuDe
    {
        [Key]
        public int MaCD { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string TenChuDe { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự.")]
        public string? MoTa { get; set; }

        [StringLength(100)]
        public string? TenChuDeNormalized { get; set; }

        public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
    }
}