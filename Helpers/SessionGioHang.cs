using System.Text.Json;
using BanSach.Models;

namespace BanSach.Helpers
{
    public class SessionGioHang
    {
        private readonly IHttpContextAccessor _http;

        public SessionGioHang(IHttpContextAccessor http)
        {
            _http = http;
        }

        public List<GioHangItem> LayGio()
        {
            var json = _http.HttpContext?.Session.GetString(SessionKeys.GioHang);
            if (string.IsNullOrEmpty(json))
            {
                return new List<GioHangItem>();
            }

            return JsonSerializer.Deserialize<List<GioHangItem>>(json) ?? new List<GioHangItem>();
        }

        public void LuuGio(List<GioHangItem> items)
        {
            _http.HttpContext?.Session.SetString(SessionKeys.GioHang, JsonSerializer.Serialize(items));
        }

        public void XoaGio()
        {
            _http.HttpContext?.Session.Remove(SessionKeys.GioHang);
        }

        public int SoLuong() => LayGio().Sum(x => x.SoLuong);

        public decimal TongTien() => LayGio().Sum(x => x.ThanhTien);
    }
}
