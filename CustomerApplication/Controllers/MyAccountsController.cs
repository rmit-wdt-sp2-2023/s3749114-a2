using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using BankLibrary.Models;
using CustomerApplication.Utilities;
using CustomerApplication.Filters;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class MyAccountsController : Controller
{
    private readonly BankContext _context;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public MyAccountsController(BankContext context) => _context = context;

    public IActionResult Index()
    {
        Customer customer = _context.Customers.Find(CustomerID);
        return View(customer);
    }

    public IActionResult Deposit(int id) => View(_context.Accounts.Find(id));

    [HttpPost]
    public IActionResult Deposit(int id, decimal amount)
    {
        var account = _context.Accounts.Find(id);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }

        // Note this code could be moved out of the controller, e.g., into the Model.
        // account.Balance += amount;
        account.Transactions.Add(
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow
            });

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
}
