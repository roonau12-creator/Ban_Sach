using System.ComponentModel.DataAnnotations;

namespace BanSach.Models;

public class Sach
{
    [Key]
    public int MaSach { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sách.")]
    [StringLength(200, ErrorMessage = "Tên sách không được vượt quá 200 ký tự.")]
    public string TenSach { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự.")]
    public string? MoTa { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục.")]
    public int MaCD { get; set; }

    public ChuDe? ChuDe { get; set; }
}
