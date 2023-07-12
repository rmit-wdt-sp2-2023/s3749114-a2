using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using BankLibrary.Models;
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
    public IActionResult Login(string loginID, string password)
    {
        Login login = _context.Logins.Find(loginID);

        if (login == null || string.IsNullOrEmpty(password) || !s_simpleHash.Verify(password, login.PasswordHash))
        { 
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
            return View(new Login
            {
                LoginID = loginID
            });
        }
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "MyAccounts");
    }

    [Route("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
