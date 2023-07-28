using Microsoft.AspNetCore.Mvc;
using CustomerApplication.ViewModels;
using BankLibrary.Models;
using CustomerApplication.Services;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Controllers;

public class LoginController : Controller
{
    private readonly LoginService _loginService;

    public LoginController(LoginService loginService) => _loginService = loginService;

    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public IActionResult SubmitLogin(LoginViewModel loginVM)
    {
        if (!ModelState.IsValid)
            return View(nameof(Login), loginVM);

        (ValidationResult error, Login login) = _loginService.Login(loginVM.LoginID, loginVM.Password);

        if (error is not null)
        {
            ModelState.AddModelError(error.MemberNames.First(), error.ErrorMessage);
            return View(nameof(Login), loginVM);
        }
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        if (login.Customer.ProfilePicture is not null)
            HttpContext.Session.SetString(nameof(Customer.ProfilePicture), login.Customer.ProfilePicture);

        return RedirectToAction("Index", "Accounts");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}