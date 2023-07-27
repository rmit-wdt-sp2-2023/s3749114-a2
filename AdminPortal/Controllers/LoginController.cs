using AdminPortal.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AdminPortal.Controllers;

[AuthorizeAdmin]
public class LoginController : Controller
{
    public IActionResult Index() => View();
}

