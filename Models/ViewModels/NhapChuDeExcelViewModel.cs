using System.ComponentModel.DataAnnotations;

namespace BanSach.Models.ViewModels
{
    public class NhapChuDeExcelViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn file Excel")]
        [Display(Name = "File Excel (.xlsx)")]
        public IFormFile? File { get; set; }

        [Display(Name = "Bỏ qua chủ đề trùng tên với chủ đề đã có")]
        public bool BoQuaTrung { get; set; } = true;

        public List<string> Loi { get; set; } = new();

        /// <summary>Thông báo không phải lỗi (vd: tất cả chủ đề trong file đều đã có).</summary>
        public string? ThongBao { get; set; }
    }
}
