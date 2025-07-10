using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TimeKeeping.Application.Attributes
{
    public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            
            // Kiểm tra đăng nhập
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            // Kiểm tra GroupFuncID = 1 (Admin)
            var groupFuncId = user.FindFirst("GroupFuncID")?.Value;
            if (groupFuncId != "1")
            {
                context.Result = new RedirectToActionResult("Forbidden", "Error", null);
                return;
            }
        }
    }
}