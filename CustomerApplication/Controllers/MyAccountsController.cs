using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using BankLibrary.Models;
using CustomerApplication.Models;
using CustomerApplication.Filters;
using BankLibrary.Utilities;
using Castle.Core.Resource;

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
        List<AccountViewModel> accountsViewModel = new();
        foreach (Account account in customer.Accounts)
        {
            accountsViewModel.Add(new AccountViewModel
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                Balance = account.CalculateBalance()
            });
        }
        return View(accountsViewModel);
    }

    public IActionResult Deposit(int id)
    {
        Account account = _context.Accounts.Find(id);
        return View(new TransactionViewModel
        {
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType,
            Balance = account.CalculateBalance()
        });
    }

    [HttpPost]
    public IActionResult Deposit(Transaction depositViewModel)
    {
        Account account = _context.Accounts.Find(depositViewModel.AccountNumber);
        if (!ModelState.IsValid)
        {
            return View(new TransactionViewModel
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                Balance = account.CalculateBalance()
            });
        }

        //Transaction transaction = account.Deposit(depositViewModel.Amount, depositViewModel.Comment);

        //if (transaction is not null)
        //{

        //}


        //return Confirm(transaction);


        //_context.Transactions.Add(transaction);
        //_context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    //public IActionResult Confirm(Transaction transaction)
    //{

    //    return View(transaction);

    //}

    //public IActionResult Confirm(Transaction transaction)
    //{

    //    return View(transaction);

    //}
}
