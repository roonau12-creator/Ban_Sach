using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BanSach.Helpers;

namespace BanSach.Models
{
    public class ThanhToan
    {
        [Key]
        public int MaTT { get; set; }

        public int MaDH { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal SoTien { get; set; }

        [Required]
        [StringLength(30)]
        public string PhuongThuc { get; set; } = PhuongThucThanhToan.COD;

        [Required]
        [StringLength(30)]
        public string TrangThai { get; set; } = TrangThaiThanhToan.ChuaThanhToan;

        [StringLength(100)]
        public string? MaGiaoDich { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public DateTime NgayCapNhat { get; set; } = DateTime.UtcNow;
        public DateTime? NgayThanhToan { get; set; }

        public DonHang? DonHang { get; set; }

        public void CapNhatTrangThai(string trangThai, decimal tongTienDon, DateTime thoiGianCapNhat, string? maGiaoDich)
        {
            if (trangThai is not TrangThaiThanhToan.DaThanhToan and not TrangThaiThanhToan.ThatBai)
            {
                throw new ArgumentException("Trạng thái thanh toán không hợp lệ.", nameof(trangThai));
            }

            if (TrangThai == TrangThaiThanhToan.DaThanhToan && trangThai != TrangThaiThanhToan.DaThanhToan)
            {
                throw new InvalidOperationException("Không thể thay đổi trạng thái của giao dịch đã thanh toán.");
            }

            if (trangThai == TrangThaiThanhToan.DaThanhToan && SoTien != tongTienDon)
            {
                throw new InvalidOperationException("Số tiền thanh toán không khớp với tổng tiền đơn hàng.");
            }

            var giaoDich = string.IsNullOrWhiteSpace(maGiaoDich)
                ? TrangThai == TrangThaiThanhToan.DaThanhToan && trangThai == TrangThaiThanhToan.DaThanhToan
                    ? MaGiaoDich
                    : null
                : maGiaoDich.Trim();
            if (giaoDich?.Length > 100)
            {
                throw new ArgumentException("Mã giao dịch không được vượt quá 100 ký tự.", nameof(maGiaoDich));
            }

            if (trangThai == TrangThaiThanhToan.DaThanhToan
                && PhuongThuc == PhuongThucThanhToan.ChuyenKhoan
                && giaoDich == null)
            {
                throw new InvalidOperationException("Cần mã giao dịch đã đối chiếu để xác nhận chuyển khoản.");
            }

            var daThayDoi = TrangThai != trangThai || MaGiaoDich != giaoDich;
            TrangThai = trangThai;
            MaGiaoDich = giaoDich;
            if (daThayDoi)
            {
                NgayCapNhat = thoiGianCapNhat;
            }

            if (trangThai == TrangThaiThanhToan.DaThanhToan)
            {
                NgayThanhToan ??= thoiGianCapNhat;
            }
            else
            {
                NgayThanhToan = null;
            }
        }
    }
}
