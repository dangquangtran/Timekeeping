using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TimeKeeping.Application.Attributes
{
    public class UserOrAdminAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            // Chỉ cần kiểm tra đăng nhập (cả Admin và User đều được phép)
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }
        }
    }
}