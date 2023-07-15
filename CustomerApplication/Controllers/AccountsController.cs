using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using BankLibrary.Models;
using CustomerApplication.Models;
using CustomerApplication.Filters;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class AccountsController : Controller
{
    private readonly BankContext _context;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public AccountsController(BankContext context) => _context = context;

    public IActionResult Index()
    {
        return View(MakeAccountsViewModel());
    }

    public IActionResult Deposit()
    {
        return View(MakeTransactionViewModel(TransactionType.Deposit));
    }

    [HttpPost]
    public IActionResult Deposit(TransactionViewModel transactionViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(MakeTransactionViewModel(TransactionType.Deposit));
        }
        Account account = _context.Accounts.Find(transactionViewModel.AccountNumber);
        transactionViewModel.AccountType = account.AccountType;
        return RedirectToAction(nameof(Confirm), transactionViewModel);
    }

    public IActionResult Confirm(TransactionViewModel transactionViewModel)
    {
        return View(transactionViewModel);
    }

    [HttpPost]
    public IActionResult Confirmed(TransactionViewModel transactionViewModel)
    {
        Account account = _context.Accounts.Find(transactionViewModel.AccountNumber);
        Transaction transaction = account.Deposit(transactionViewModel.Amount, transactionViewModel.Comment);
        _context.Transactions.Add(transaction);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    public TransactionViewModel MakeTransactionViewModel(TransactionType transactionType)
    {
        return new TransactionViewModel
        {
            TransactionType = transactionType,
            AccountsViewModel = MakeAccountsViewModel()
        };
    }

    public List<AccountViewModel> MakeAccountsViewModel()
    {
        List<Account> accounts =
            _context.Accounts.Where(x => x.CustomerID == CustomerID).OrderBy(x => x.AccountNumber).ToList();
        List<AccountViewModel> accountsViewModel = new();
        foreach (Account account in accounts)
        {
            accountsViewModel.Add(new AccountViewModel
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                Balance = account.CalculateBalance()
            });
        }
        return accountsViewModel;
    }
}
