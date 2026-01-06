using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoPartsPOS.Filters
{
    public class AuthorizePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permission;

        public AuthorizePermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;

            // مش مسجل دخول
            if (!httpContext.Session.Keys.Contains("Permissions"))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var permissions = httpContext.Session.GetString("Permissions");

            // مفيش صلاحية
            if (string.IsNullOrEmpty(permissions) || !permissions.Split(',').Contains(_permission))
            {
                context.Result = new ForbidResult(); // 403
            }
        }
    }
}
