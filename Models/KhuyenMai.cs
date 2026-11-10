using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BanSach.Helpers;

namespace BanSach.Models
{
    public class KhuyenMai : IValidatableObject
    {
        [Key]
        public int MaKM { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã khuyến mãi")]
        [StringLength(40)]
        [RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "Mã chỉ được chứa chữ cái, chữ số, dấu gạch ngang và gạch dưới.")]
        [Display(Name = "Mã khuyến mãi")]
        public string MaCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên chương trình")]
        [StringLength(150)]
        [Display(Name = "Tên chương trình")]
        public string TenKhuyenMai { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Loại giảm giá")]
        public string LoaiGiamGia { get; set; } = LoaiGiamGiaKhuyenMai.PhanTram;

        [Column(TypeName = "numeric(18,2)")]
        [Range(typeof(decimal), "0.01", "1000000000", ErrorMessage = "Mức giảm phải lớn hơn 0.")]
        [Display(Name = "Mức giảm")]
        public decimal GiaTriGiam { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        [Range(typeof(decimal), "0.01", "1000000000", ErrorMessage = "Mức giảm tối đa phải lớn hơn 0.")]
        [Display(Name = "Giảm tối đa")]
        public decimal? GiamToiDa { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "Giá trị đơn hàng tối thiểu không hợp lệ.")]
        [Display(Name = "Đơn hàng tối thiểu")]
        public decimal DonHangToiThieu { get; set; }

        [Display(Name = "Bắt đầu (UTC)")]
        public DateTime NgayBatDau { get; set; } = DateTime.UtcNow;

        [Display(Name = "Kết thúc (UTC)")]
        public DateTime NgayKetThuc { get; set; } = DateTime.UtcNow.AddDays(7);

        [Display(Name = "Đang hoạt động")]
        public bool DangHoatDong { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NgayKetThuc <= NgayBatDau)
            {
                yield return new ValidationResult(
                    "Thời gian kết thúc phải sau thời gian bắt đầu.",
                    new[] { nameof(NgayKetThuc) });
            }

            if (LoaiGiamGia == LoaiGiamGiaKhuyenMai.PhanTram && GiaTriGiam > 100)
            {
                yield return new ValidationResult(
                    "Mức giảm phần trăm không được vượt quá 100%.",
                    new[] { nameof(GiaTriGiam) });
            }
            else if (LoaiGiamGia != LoaiGiamGiaKhuyenMai.PhanTram
                && LoaiGiamGia != LoaiGiamGiaKhuyenMai.SoTien)
            {
                yield return new ValidationResult(
                    "Loại giảm giá không hợp lệ.",
                    new[] { nameof(LoaiGiamGia) });
            }

            if (LoaiGiamGia != LoaiGiamGiaKhuyenMai.PhanTram && GiamToiDa.HasValue)
            {
                yield return new ValidationResult(
                    "Chỉ áp dụng mức giảm tối đa cho chương trình giảm theo phần trăm.",
                    new[] { nameof(GiamToiDa) });
            }
        }
    }
}
