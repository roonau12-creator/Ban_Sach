namespace BanSach.Models
{
    public class GioHangItem
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public string? AnhBia { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => GiaBan * SoLuong;
    }
}
