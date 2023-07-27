using Microsoft.AspNetCore.Mvc;
using AdminPortal.Filters;

namespace AdminPortal.Controllers;

[AuthorizeAdmin]
public class CustomerController : Controller
{
    public IActionResult Index() => View();
}

