using CustomerApplication.Filters;
using CustomerApplication.Models;
using CustomerApplication.Services;
using CustomerApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class StatementsController : Controller
{
    private readonly TransactionService _transactionService;
    private readonly AccountService _accountService;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public StatementsController(TransactionService transactionService, AccountService accountService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
    }

    public IActionResult Index() => View(StatementsVM());

    [HttpPost]
    public IActionResult Statements(StatementsViewModel statementsVM)
    {
        if (!ModelState.IsValid)
            return View(nameof(Index), statementsVM);
        return RedirectToAction(nameof(Statements), new { statementsVM.AccountNumber });
    }

    public IActionResult Statements(int accountNumber, int page = 1)
    {
        StatementsViewModel statementsVM = StatementsVM();
        if (statementsVM.AccountsViewModel.FindIndex(x => x.AccountNumber == accountNumber) < 0)
        {
            ModelState.AddModelError("AccountNumber", "You must select a valid account.");
            return View(nameof(Index), statementsVM);
        }                  
        const int pageSize = 4;
        IPagedList<Transaction> pagedList = _transactionService.GetPagedTransactions(accountNumber, page, pageSize);
        statementsVM.AccountNumber = accountNumber;
        statementsVM.Transactions = pagedList;
        return View(nameof(Index), statementsVM);
    }

    private StatementsViewModel StatementsVM()
    {
        return new StatementsViewModel
        {
            AccountsViewModel = AccountsVM()
        };
    }

    private List<AccountViewModel> AccountsVM()
    {
        List<AccountViewModel> accountsVM = new();
        foreach (Account account in _accountService.GetAccounts(CustomerID))
        {
            accountsVM.Add(new AccountViewModel
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                Balance = account.Balance(),
                AvailableBalance = account.AvailableBalance()
            });
        }
        return accountsVM;
    }
}