using System.ComponentModel.DataAnnotations;
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
        ViewBag.DisplayBlocked = false;
        ViewBag.DisplayFailed = false;

        List<BillPayViewModel> viewModels = new();

        foreach (BillPay b in _bankService.GetBillPays(CustomerID))
        {
            viewModels.Add(new BillPayViewModel()
            {
                BillPayID = b.BillPayID,
                AccountNumber = b.AccountNumber,
                PayeeID = b.PayeeID,
                Amount = b.Amount,
                ScheduledTimeLocal = b.ScheduledTimeUtc.ToLocalTime(),
                Period = b.Period,
                BillPayStatus = b.BillPayStatus
            });
            if (b.BillPayStatus == BillPayStatus.Failed)
                ViewBag.DisplayFailed = true;
            else if (b.BillPayStatus == BillPayStatus.Blocked)
                ViewBag.DisplayFailed = true;
        }
        return View(viewModels);
    }

    public BillPayScheduleViewModel BillPayScheduleViewModel()
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
        BillPayScheduleViewModel viewModel = new()
        {
            AccountViewModels = viewModels
        };
        return viewModel;
    }

    public IActionResult Schedule()
    {
        return View(BillPayScheduleViewModel());
    }

    [HttpPost]
    public IActionResult Confirm(BillPayScheduleViewModel viewModel)
    {
        List<ValidationResult> errors = _bankService.ConfirmBillPay(
            viewModel.AccountNumber.GetValueOrDefault(), viewModel.PayeeID.GetValueOrDefault(),
            viewModel.Amount.GetValueOrDefault(), viewModel.ScheduledTimeLocal.GetValueOrDefault(), viewModel.Period);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Schedule), BillPayScheduleViewModel());

        Account account = _bankService.GetAccount(viewModel.AccountNumber.GetValueOrDefault());
        viewModel.AccountType = account.AccountType;

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Submit(BillPayScheduleViewModel viewModel)
    {
        List<ValidationResult> errors = _bankService.SubmitBillPay(
            viewModel.AccountNumber.GetValueOrDefault(), viewModel.PayeeID.GetValueOrDefault(),
            viewModel.Amount.GetValueOrDefault(), viewModel.ScheduledTimeLocal.GetValueOrDefault(), viewModel.Period);

        if (errors is null)
            return View("Success", viewModel);

        foreach (ValidationResult e in errors)
            ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        return View(nameof(Schedule), BillPayScheduleViewModel());
    }

    public IActionResult Cancel(int id)
    {
        BillPay billPay = _bankService.GetBillPay(id);

        return View(new BillPayViewModel()
        {
            BillPayID = billPay.BillPayID,
            AccountNumber = billPay.AccountNumber,
            PayeeID = billPay.PayeeID,
            Amount = billPay.Amount,
            ScheduledTimeLocal = billPay.ScheduledTimeUtc.ToLocalTime(),
            Period = billPay.Period,
            BillPayStatus = billPay.BillPayStatus
        });
    }

    [HttpPost]
    public IActionResult Cancel(BillPayViewModel viewModel)
    {

        ValidationResult error = _bankService.CancelBillPay(viewModel.BillPayID);
        if (error is not null)
        {
            Console.WriteLine("BILL PAY ID " + viewModel.BillPayID);
            Console.WriteLine("ERROR");
        }

        return RedirectToAction(nameof(Index));
    }
}