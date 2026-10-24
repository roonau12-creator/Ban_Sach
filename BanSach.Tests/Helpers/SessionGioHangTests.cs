using BanSach.Helpers;
using BanSach.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BanSach.Tests.Helpers
{
    public class SessionGioHangTests
    {
        private static SessionGioHang TaoSession()
        {
            var accessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext
                {
                    Session = new FakeSession()
                }
            };
            accessor.HttpContext!.RequestServices =
                new ServiceCollection().BuildServiceProvider();
            return new SessionGioHang(accessor);
        }

        [Fact]
        public void LayGio_SessionRong_TraVeDanhSachRong()
        {
            var gio = TaoSession();

            Assert.Empty(gio.LayGio());
        }

        [Fact]
        public void SoLuong_SessionRong_La0()
        {
            var gio = TaoSession();

            Assert.Equal(0, gio.SoLuong());
        }

        [Fact]
        public void TongTien_SessionRong_La0()
        {
            var gio = TaoSession();

            Assert.Equal(0m, gio.TongTien());
        }

        [Fact]
        public void LuuGio_LayGio_TraVeDuLieuGiongNhau()
        {
            var gio = TaoSession();
            var items = new List<GioHangItem>
            {
                new() { MaSach = 1, TenSach = "Sách A", GiaBan = 100_000m, SoLuong = 2 },
                new() { MaSach = 2, TenSach = "Sách B", GiaBan = 50_000m, SoLuong = 1 }
            };

            gio.LuuGio(items);
            var ketQua = gio.LayGio();

            Assert.Equal(2, ketQua.Count);
            Assert.Equal("Sách A", ketQua[0].TenSach);
            Assert.Equal(100_000m, ketQua[0].GiaBan);
            Assert.Equal(2, ketQua[0].SoLuong);
        }

        [Fact]
        public void SoLuong_TinhTongSoLuongCacMatHang()
        {
            var gio = TaoSession();
            gio.LuuGio(new List<GioHangItem>
            {
                new() { GiaBan = 10_000m, SoLuong = 3 },
                new() { GiaBan = 20_000m, SoLuong = 4 }
            });

            Assert.Equal(7, gio.SoLuong());
        }

        [Fact]
        public void TongTien_TinhTongThanhTienCacMatHang()
        {
            var gio = TaoSession();
            gio.LuuGio(new List<GioHangItem>
            {
                new() { GiaBan = 100_000m, SoLuong = 2 },
                new() { GiaBan = 50_000m, SoLuong = 1 }
            });

            Assert.Equal(250_000m, gio.TongTien());
        }

        [Fact]
        public void XoaGio_GioTrongLaRong_SoLuongVe0()
        {
            var gio = TaoSession();
            gio.LuuGio(new List<GioHangItem> { new() { GiaBan = 10_000m, SoLuong = 5 } });

            gio.XoaGio();

            Assert.Empty(gio.LayGio());
            Assert.Equal(0, gio.SoLuong());
        }

        private sealed class FakeSession : ISession
        {
            private readonly Dictionary<string, byte[]> _duLieu = new();

            public bool IsAvailable => true;
            public string Id { get; } = Guid.NewGuid().ToString("N");
            public IEnumerable<string> Keys => _duLieu.Keys;

            public Task LoadAsync(CancellationToken cancellationToken) => Task.CompletedTask;
            public Task CommitAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            public void Clear() => _duLieu.Clear();

            public bool TryGetValue(string key, out byte[] value) =>
                _duLieu.TryGetValue(key, out value!);

            public void Set(string key, byte[] value) => _duLieu[key] = value;

            public void Remove(string key) => _duLieu.Remove(key);
        }
    }
}
