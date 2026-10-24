using BanSach.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BanSach.Filters
{
    public class KiemTraDangNhapAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var maTK = context.HttpContext.Session.GetInt32(SessionKeys.MaTK);
            if (maTK == null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new
                {
                    area = "Customer",
                    returnUrl
                });
            }
        }
    }
}
