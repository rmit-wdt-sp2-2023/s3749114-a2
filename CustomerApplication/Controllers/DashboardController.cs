using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using BankLibrary.Models;
using CustomerApplication.Models;
using CustomerApplication.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using System.Transactions;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class DashboardController : Controller
{
    private readonly BankContext _context;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public DashboardController(BankContext context) => _context = context;

    public IActionResult Index() => View(MakeAccountsViewModel());

    // DEPOSIT

    public IActionResult Deposit() => View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Deposit));

    [HttpPost]
    public IActionResult Deposit(TransactionViewModel transactionViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Deposit));
        }
        Account account = _context.Accounts.Find(transactionViewModel.AccountNumber);
        transactionViewModel.AccountType = account.AccountType;
        return RedirectToAction(nameof(Confirm), transactionViewModel);
    }

    // WITHDRAW

    public IActionResult Withdraw() => View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Withdraw));

    [HttpPost]
    public IActionResult Withdraw(TransactionViewModel transactionViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Deposit));
        }
        Account account = _context.Accounts.Find(transactionViewModel.AccountNumber);
        transactionViewModel.AccountType = account.AccountType;
        return RedirectToAction(nameof(Confirm), transactionViewModel);
    }

    // RESULT

    [HttpPost]
    public IActionResult Result(TransactionViewModel transactionViewModel)
    {
        Account account = _context.Accounts.Find(transactionViewModel.AccountNumber);

        if (transactionViewModel.TransactionType == TransactionType.Deposit)
        {
            transactionViewModel.TransactionResult = true;
            Transaction transaction = account.Deposit(transactionViewModel.Amount, transactionViewModel.Comment);
            _context.Transactions.Add(transaction);
        }
        else if (transactionViewModel.TransactionType == TransactionType.Withdraw)
        {
            (List<Transaction> transactions, string message) =
                account.Withdraw(transactionViewModel.Amount, transactionViewModel.Comment);

            if (transactions is null)
                ModelState.AddModelError(nameof(transactionViewModel.TransactionResult), message);
            else
            {
                transactionViewModel.TransactionResult = true;
                foreach (Transaction transaction in transactions)
                    _context.Transactions.Add(transaction);
                _context.SaveChanges();
            }
        }
        return View(transactionViewModel);
    }

    // CONFIRM

    public IActionResult Confirm(TransactionViewModel transactionViewModel) => View(transactionViewModel);

    //[HttpPost]
    //public IActionResult Confirmed(TransactionViewModel transactionViewModel)
    //{
    //    Account account = _context.Accounts.Find(transactionViewModel.AccountNumber);

    //    if (transactionViewModel.TransactionType == TransactionType.Deposit)
    //    {
    //        Transaction transaction = account.Deposit(transactionViewModel.Amount, transactionViewModel.Comment);
    //        _context.Transactions.Add(transaction);
    //    }
    //    else if (transactionViewModel.TransactionType == TransactionType.Withdraw)
    //    {
    //        (List<Transaction> transactions, string message) =
    //            account.Withdraw(transactionViewModel.Amount, transactionViewModel.Comment);

    //        foreach (Transaction transaction in transactions)
    //            _context.Transactions.Add(transaction);
    //    }
    //    _context.SaveChanges();
    //    return RedirectToAction(nameof(Receipt));
    //}


    // VIEW MODEL CREATION

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
                Balance = account.CalculateBalance(),
                AvailableBalance = account.AvailableBalance()
            }); 
        }
        return accountsViewModel;
    }
}
