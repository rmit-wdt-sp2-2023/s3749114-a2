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
    private readonly BankService _bankService;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public StatementsController(BankService bankService) => _bankService = bankService;

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

        IPagedList<Transaction> pagedList = _bankService.GetPagedTransactions(accountNumber, page, pageSize);

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
        List<Account> accounts = _bankService.GetAccounts(CustomerID);
        List<AccountViewModel> accountsVM = new();
        foreach (Account account in accounts)
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