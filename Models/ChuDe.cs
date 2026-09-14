using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class ChuDe
    {
        [Key]
        public int MaCD { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chủ đề")]
        [StringLength(100)]
        [Display(Name = "Tên chủ đề")]
        public string TenChuDe { get; set; } = string.Empty;

        public ICollection<Sach> Sachs { get; set; } = new List<Sach>();
    }
}
