using System.ComponentModel.DataAnnotations;

namespace BanSach.Models.ViewModels
{
    public class NhapSachExcelViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn file Excel")]
        [Display(Name = "File Excel (.xlsx)")]
        public IFormFile? File { get; set; }

        [Display(Name = "Tự tạo chủ đề, nhóm sách, nhà xuất bản, tác giả nếu chưa có")]
        public bool TuTaoDanhMuc { get; set; } = true;

        [Display(Name = "Bỏ qua sách trùng tên với sách đã có")]
        public bool BoQuaTrung { get; set; } = true;

        public List<string> Loi { get; set; } = new();
    }
}
