using BanSach.Helpers;
using BanSach.Models;
using BanSach.Services.Email;

namespace BanSach.Tests.Services
{
    public class OrderEmailTemplateTests
    {
        private static DonHang CreateOrder()
        {
            return new DonHang
            {
                MaDH = 43,
                NgayDat = new DateTime(2026, 11, 16, 10, 0, 0, DateTimeKind.Utc),
                TrangThai = TrangThaiDonHang.ChoXacNhan,
                TongTien = 150_000m,
                DiaChiGiao = "123 Đường Sách",
                SoDienThoaiGiao = "0900000000",
                KhachHang = new KhachHang { HoTen = "Nguyễn An" },
                ChiTietDonHangs =
                {
                    new ChiTietDonHang
                    {
                        MaSach = 7,
                        SoLuong = 2,
                        DonGia = 75_000m,
                        Sach = new Sach { TenSach = "Lập trình C#" }
                    }
                }
            };
        }

        [Fact]
        public void Confirmation_IncludesOrderAndProductDetails()
        {
            var email = OrderEmailTemplate.Confirmation(CreateOrder());

            Assert.Equal("Xác nhận đơn hàng #43", email.Subject);
            Assert.Contains("Mã đơn hàng: #43", email.Body);
            Assert.Contains("Lập trình C#", email.Body);
            Assert.Contains("Số lượng: 2", email.Body);
            Assert.Contains("Tổng tiền: 150.000 ₫", email.Body);
        }

        [Fact]
        public void StatusChanged_IncludesOrderAndPreviousAndNewStatuses()
        {
            var email = OrderEmailTemplate.StatusChanged(
                CreateOrder(),
                TrangThaiDonHang.ChoXacNhan,
                TrangThaiDonHang.DangGiao);

            Assert.Contains("#43", email.Subject);
            Assert.Contains("Chờ xác nhận", email.Body);
            Assert.Contains("Đang giao", email.Body);
            Assert.Contains("Lập trình C#", email.Body);
        }

        [Fact]
        public void ConfirmationOutbox_UsesStableKeyForTheOrderEvent()
        {
            var order = CreateOrder();

            var first = OrderEmailOutboxFactory.Confirmation(order, "customer@example.com");
            var duplicate = OrderEmailOutboxFactory.Confirmation(order, "customer@example.com");

            Assert.Equal(first.EventKey, duplicate.EventKey);
            Assert.Equal("customer@example.com", first.Recipient);
        }

        [Fact]
        public void StatusOutbox_UsesStatusVersionToDeduplicateTheSameEvent()
        {
            var order = CreateOrder();
            order.TrangThaiVersion = 2;

            var first = OrderEmailOutboxFactory.StatusChanged(
                order, TrangThaiDonHang.ChoXacNhan, TrangThaiDonHang.DangGiao, "customer@example.com");
            var duplicate = OrderEmailOutboxFactory.StatusChanged(
                order, TrangThaiDonHang.ChoXacNhan, TrangThaiDonHang.DangGiao, "customer@example.com");

            Assert.Equal(first.EventKey, duplicate.EventKey);
        }
    }
}
