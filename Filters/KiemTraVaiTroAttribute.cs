using BanSach.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BanSach.Filters
{
    public class KiemTraVaiTroAttribute : ActionFilterAttribute
    {
        private readonly string _vaiTro;

        public KiemTraVaiTroAttribute(string vaiTro)
        {
            _vaiTro = vaiTro;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maTK = context.HttpContext.Session.GetInt32(SessionKeys.MaTK);
            var vaiTro = context.HttpContext.Session.GetString(SessionKeys.VaiTro);

            if (maTK == null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new
                {
                    area = "Customer",
                    returnUrl
                });
                return;
            }

            if (!string.Equals(vaiTro, _vaiTro, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("Index", "Home", new { area = "Customer" });
            }
        }
    }
}
