using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using BankLibrary.Models;
using CustomerApplication.Models;
using SimpleHashing.Net;

namespace CustomerApplication.Controllers;

[Route("Login")]
public class LoginController : Controller
{
    private static readonly ISimpleHash s_simpleHash = new SimpleHash();

    private readonly BankContext _context;

    public LoginController(BankContext context) => _context = context;

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(LoginViewModel loginViewModel)
    {
        if (ModelState.IsValid)
        {
            Login login = _context.Logins.Find(loginViewModel.LoginID);
            if (login is not null)
            {
                if (s_simpleHash.Verify(loginViewModel.Password, login.PasswordHash))
                {
                    HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
                    HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);
                    return RedirectToAction("Index", "MyAccounts");
                }
            }
        }
        ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
        return View(new LoginViewModel());
    }

    [Route("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
