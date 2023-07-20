using Castle.Core.Resource;
using CustomerApplication.Filters;
using CustomerApplication.Models;
using CustomerApplication.Services;
using CustomerApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class BillPayController : Controller
{
    private readonly BankService _bankService;
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public BillPayController(BankService bankService) => _bankService = bankService;

    public IActionResult Index()
    {
        List<BillPayViewModel> viewModels = new();
        foreach (BillPay b in _bankService.GetBillPays(CustomerID))
        {
            viewModels.Add(new BillPayViewModel()
            {
                BillPayID = b.BillPayID,
                AccountNumber = b.AccountNumber,
                PayeeID = b.PayeeID,
                Amount = b.Amount,
                ScheduledTimeUtc = b.ScheduledTimeUtc,
                Period = b.Period,
                BillPayStatus = b.BillPayStatus
            });
        }
        return View(viewModels);
    }

    public IActionResult Schedule()
    {
        List<AccountViewModel> viewModels = new();

        foreach (Account a in _bankService.GetAccounts(CustomerID))
        {
            viewModels.Add(new AccountViewModel
            {
                AccountNumber = a.AccountNumber,
                AccountType = a.AccountType,
                Balance = a.Balance(),
                AvailableBalance = a.AvailableBalance()
            });
        }

        BillPayViewModel viewModel = new()
        {
            AccountViewModels = viewModels
        };

        return View(viewModel);
    }

    public IActionResult Confirm()
    {
        return View();
    }

    public IActionResult Cancel()
    {
        return View();
    }


    public int BillPayID { get; set; }

    public int AccountNumber { get; set; }

    public int PayeeID { get; set; }

    public decimal Amount { get; set; }

    public DateTime ScheduledTimeUtc { get; set; }

    public Period Period { get; set; }

    public BillPayStatus BillPayStatus { get; set; }

    public List<AccountViewModel> AccountsViewModel()
    {
        List<Account> accounts = _bankService.GetAccounts(CustomerID);
        List<AccountViewModel> viewModel = new();
        foreach (Account account in accounts)
        {
            viewModel.Add(new AccountViewModel
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                Balance = account.Balance(),
                AvailableBalance = account.AvailableBalance()
            });
        }
        return viewModel;
    }
}