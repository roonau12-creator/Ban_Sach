namespace BanSach.Helpers
{
    public static class VaiTroNguoiDung
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
    }

    public static class TrangThaiDonHang
    {
        public const string ChoXacNhan = "ChoXacNhan";
        public const string DaXacNhan = "DaXacNhan";
        public const string DangGiao = "DangGiao";
        public const string DaGiao = "DaGiao";
        public const string DaHuy = "DaHuy";
    }

    public static class PhuongThucThanhToan
    {
        public const string COD = "COD";
        public const string ChuyenKhoan = "ChuyenKhoan";
    }

    public static class TrangThaiThanhToan
    {
        public const string ChuaThanhToan = "ChuaThanhToan";
        public const string DaThanhToan = "DaThanhToan";
    }

    public static class TrangThaiGiaoHang
    {
        public const string ChoGiao = "ChoGiao";
        public const string DangGiao = "DangGiao";
        public const string DaGiao = "DaGiao";
    }

    public static class SessionKeys
    {
        public const string MaTK = "MaTK";
        public const string TenDangNhap = "TenDangNhap";
        public const string HoTen = "HoTen";
        public const string VaiTro = "VaiTro";
        public const string MaKH = "MaKH";
        public const string GioHang = "GioHang";
    }

    public static class HienThi
    {
        public static string TrangThaiDon(string? value) => value switch
        {
            TrangThaiDonHang.ChoXacNhan => "Chờ xác nhận",
            TrangThaiDonHang.DaXacNhan => "Đã xác nhận",
            TrangThaiDonHang.DangGiao => "Đang giao",
            TrangThaiDonHang.DaGiao => "Đã giao",
            TrangThaiDonHang.DaHuy => "Đã hủy",
            _ => value ?? ""
        };

        public static string BadgeTrangThaiDon(string? value) => value switch
        {
            TrangThaiDonHang.ChoXacNhan => "bg-warning text-dark",
            TrangThaiDonHang.DaXacNhan => "bg-info text-white",
            TrangThaiDonHang.DangGiao => "bg-primary",
            TrangThaiDonHang.DaGiao => "bg-success",
            TrangThaiDonHang.DaHuy => "bg-secondary",
            _ => "bg-light text-dark"
        };

        public static string ThanhToan(string? value) => value switch
        {
            TrangThaiThanhToan.DaThanhToan => "Đã thanh toán",
            TrangThaiThanhToan.ChuaThanhToan => "Chưa thanh toán",
            _ => value ?? ""
        };

        public static string PhuongThuc(string? value) => value switch
        {
            PhuongThucThanhToan.COD => "Thanh toán khi nhận hàng",
            PhuongThucThanhToan.ChuyenKhoan => "Chuyển khoản",
            _ => value ?? ""
        };

        public static string GiaoHang(string? value) => value switch
        {
            TrangThaiGiaoHang.ChoGiao => "Chờ giao",
            TrangThaiGiaoHang.DangGiao => "Đang giao",
            TrangThaiGiaoHang.DaGiao => "Đã giao",
            _ => value ?? ""
        };

        public static string Tien(decimal value) => string.Format("{0:N0} ₫", value);
    }
}
