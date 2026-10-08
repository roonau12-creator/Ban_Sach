using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BanSach.Models.ViewModels
{
    public class SachFormViewModel
    {
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

        [Range(0, double.MaxValue, ErrorMessage = "Giá bán không hợp lệ")]
        [Display(Name = "Giá bán")]
        public decimal GiaBan { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ")]
        [Display(Name = "Số lượng tồn")]
        public int SoLuongTon { get; set; }

        public string? AnhBia { get; set; }

        [Display(Name = "Ảnh bìa")]
        public IFormFile? AnhBiaFile { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Năm xuất bản")]
        public int? NamXB { get; set; }

        [Display(Name = "Sách mới")]
        public bool SachMoi { get; set; }

        [Display(Name = "Nổi bật")]
        public bool NoiBat { get; set; }

        [Display(Name = "Tác giả")]
        public List<int> MaTacGias { get; set; } = new();

        public IEnumerable<SelectListItem> ChuDes { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> NhomSachs { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> NhaXuatBans { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> TacGias { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
