using Microsoft.AspNetCore.Mvc;
using AdminPortal.ViewModels;

namespace AdminPortal.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View(new LoginViewModel());

    [HttpPost]
    public IActionResult SubmitLogin(LoginViewModel loginVM)
    {
        if (!ModelState.IsValid)
            return View(nameof(Index), loginVM);

        if (loginVM.Username != "admin" && loginVM.Password != "admin")
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(nameof(Index), loginVM);
        }
        HttpContext.Session.SetString("Username", loginVM.Username);

        return RedirectToAction(nameof(Index), "Dashboard");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Index));
    }
}