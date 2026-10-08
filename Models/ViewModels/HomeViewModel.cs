namespace BanSach.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Sach> SachMois { get; set; } = new();
        public List<Sach> SachNoiBats { get; set; } = new();
        public List<ChuDe> ChuDes { get; set; } = new();
        public string? TuKhoa { get; set; }
    }

    public class DashboardViewModel
    {
        public int TongSach { get; set; }
        public int TongKhachHang { get; set; }
        public int TongDonHang { get; set; }
        public decimal DoanhThu { get; set; }
        public List<DonHang> DonHangMois { get; set; } = new();
        public List<SachBanChayItem> SachBanChays { get; set; } = new();
    }

    public class SachBanChayItem
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public int SoLuongBan { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class ThongKeViewModel
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public int TongDon { get; set; }
        public decimal DoanhThu { get; set; }
        public Dictionary<string, int> DonTheoTrangThai { get; set; } = new();
        public List<DonHang> DonHangs { get; set; } = new();
    }
}
