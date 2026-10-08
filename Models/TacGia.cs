using System.ComponentModel.DataAnnotations;

namespace BanSach.Models
{
    public class TacGia
    {
        [Key]
        public int MaTG { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tác giả")]
        [StringLength(150)]
        [Display(Name = "Tên tác giả")]
        public string TenTacGia { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Tiểu sử")]
        public string? TieuSu { get; set; }

        public ICollection<SachTacGia> SachTacGias { get; set; } = new List<SachTacGia>();
    }
}
