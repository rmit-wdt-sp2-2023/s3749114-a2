using Microsoft.AspNetCore.Mvc;
using CustomerApplication.ViewModels;
using CustomerApplication.Models;
using CustomerApplication.Services;

namespace CustomerApplication.Controllers;

public class LoginController : Controller
{
    private readonly BankService _bankService;

    public LoginController(BankService bankService) => _bankService = bankService;

    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public IActionResult SubmitLogin(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(nameof(Login), viewModel);

        Login login = _bankService.Login(viewModel.LoginID, viewModel.Password);

        if (login is null)
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(nameof(Login), viewModel);
        }
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        if (login.Customer.ProfilePicture is not null)
        {
            HttpContext.Session.SetString(nameof(Customer.ProfilePicture), login.Customer.ProfilePicture);
        }
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}