using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BanSach.Models.ViewModels
{
    public class DatHangViewModel
    {
        [Display(Name = "Địa chỉ giao hàng")]
        public int MaDC { get; set; }

        [Display(Name = "Số điện thoại")]
        public int MaSDT { get; set; }

        [Required]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = Helpers.PhuongThucThanhToan.COD;

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public List<GioHangItem> Items { get; set; } = new();
        public decimal TongTien { get; set; }
        public IEnumerable<SelectListItem> DiaChis { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> SoDienThoais { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
