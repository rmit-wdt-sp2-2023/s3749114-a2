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

    public BillPayViewModel BillPayViewModel()
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

        return viewModel;
    }

    public IActionResult Schedule()
    {
        return View(BillPayViewModel());
    }

    [HttpPost]
    public IActionResult Confirm(BillPayViewModel viewModel)
    {

        // if valid
        return View(viewModel);

        // if invalid
        //return View(nameof(Schedule), BillPayViewModel());
    }

    public IActionResult Cancel()
    {
        return View();
    }



}