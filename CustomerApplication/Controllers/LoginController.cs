﻿using Microsoft.AspNetCore.Mvc;
using CustomerApplication.ViewModels;
using CustomerApplication.Models;
using CustomerApplication.Services;

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

        Login login = _loginService.Login(loginVM.LoginID, loginVM.Password);

        if (login is null)
        {
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
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