using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BankLibrary.Models;

namespace CustomerApplication.Filters;

// When [AuthorizeCustomer] is applied, redirects 
// to home page if there is no current session.

public class AuthorizeCustomerAttribute : Attribute, IAuthorizationFilter
{
    // Adaped from Bolger M (2023).

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        int? customerID = context.HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
        if(!customerID.HasValue)
            context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}
