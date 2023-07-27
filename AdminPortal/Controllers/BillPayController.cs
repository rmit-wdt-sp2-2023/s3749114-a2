using Microsoft.AspNetCore.Mvc;
using AdminPortal.Filters;

namespace AdminPortal.Controllers;

[AuthorizeAdmin]
public class BillPayController : Controller
{
    public IActionResult Index() => View();
}

