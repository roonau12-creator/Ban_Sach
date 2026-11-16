using System.Globalization;
using System.Text;
using BanSach.Helpers;
using BanSach.Models;

namespace BanSach.Services.Email
{
    public static class OrderEmailTemplate
    {
        private static readonly CultureInfo VietnameseCulture = CultureInfo.GetCultureInfo("vi-VN");

        public static (string Subject, string Body) Confirmation(DonHang order)
        {
            var subject = $"Xác nhận đơn hàng #{order.MaDH}";
            var body = new StringBuilder()
                .AppendLine($"Xin chào {order.KhachHang?.HoTen ?? "Quý khách"},")
                .AppendLine()
                .AppendLine("Cảm ơn bạn đã đặt hàng tại BanSach. Đơn hàng của bạn đã được ghi nhận.")
                .Append(OrderDetails(order))
                .ToString();

            return (subject, body);
        }

        public static (string Subject, string Body) StatusChanged(
            DonHang order,
            string previousStatus,
            string newStatus)
        {
            var subject = $"Cập nhật đơn hàng #{order.MaDH}: {HienThi.TrangThaiDon(newStatus)}";
            var body = new StringBuilder()
                .AppendLine($"Xin chào {order.KhachHang?.HoTen ?? "Quý khách"},")
                .AppendLine()
                .AppendLine(
                    $"Trạng thái đơn hàng #{order.MaDH} đã thay đổi từ " +
                    $"\"{HienThi.TrangThaiDon(previousStatus)}\" thành \"{HienThi.TrangThaiDon(newStatus)}\".")
                .Append(OrderDetails(order))
                .ToString();

            return (subject, body);
        }

        private static string OrderDetails(DonHang order)
        {
            var body = new StringBuilder()
                .AppendLine()
                .AppendLine($"Mã đơn hàng: #{order.MaDH}")
                .AppendLine($"Ngày đặt: {order.NgayDat.ToLocalTime():dd/MM/yyyy HH:mm}")
                .AppendLine($"Trạng thái: {HienThi.TrangThaiDon(order.TrangThai)}")
                .AppendLine("Sản phẩm:");

            foreach (var item in order.ChiTietDonHangs)
            {
                var name = item.Sach?.TenSach ?? $"Sách #{item.MaSach}";
                body.AppendLine(
                    $"- {name} | Số lượng: {item.SoLuong} | " +
                    $"Đơn giá: {item.DonGia.ToString("N0", VietnameseCulture)} ₫ | " +
                    $"Thành tiền: {(item.DonGia * item.SoLuong).ToString("N0", VietnameseCulture)} ₫");
            }

            body.AppendLine($"Tổng tiền: {order.TongTien.ToString("N0", VietnameseCulture)} ₫")
                .AppendLine($"Địa chỉ giao hàng: {order.DiaChiGiao}")
                .AppendLine($"Số điện thoại: {order.SoDienThoaiGiao}")
                .AppendLine()
                .AppendLine("BanSach");

            return body.ToString();
        }
    }
}
