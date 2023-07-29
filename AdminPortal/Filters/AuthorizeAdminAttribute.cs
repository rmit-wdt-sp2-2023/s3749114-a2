using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AdminPortal.Filters;

public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string username = context.HttpContext.Session.GetString("Username");
        if (username is null)
            context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}