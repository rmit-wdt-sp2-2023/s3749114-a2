using AdminPortal.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AdminPortal.Controllers;

[AuthorizeAdmin]
public class DashboardController : Controller
{
    public IActionResult Index() => View();
}

