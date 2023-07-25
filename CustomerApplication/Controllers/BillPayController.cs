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
        List<BillPayViewModel> billPayVM = BillPaysVM();

        (bool displayBlocked, bool displayFailed) = CheckBlockedAndFailed(billPayVM);

        ViewBag.DisplayBlocked = displayBlocked;
        ViewBag.DisplayFailed = displayFailed;
        ViewBag.DisplaySuccess = false;
        ViewBag.DisplayCancelled = false;

        return View(billPayVM);
    }

    public IActionResult Schedule() => View(BillPayScheduleVM());

    [HttpPost]
    public IActionResult ConfirmSchedule(BillPayScheduleViewModel billPayScheduleVM)
    {
        List<ValidationResult> errors = _bankService.ConfirmBillPay(billPayScheduleVM.AccountNumber.GetValueOrDefault(),
            billPayScheduleVM.PayeeID.GetValueOrDefault(), billPayScheduleVM.Amount.GetValueOrDefault(),
            billPayScheduleVM.ScheduledTimeLocal.GetValueOrDefault(), billPayScheduleVM.Period.GetValueOrDefault());

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Schedule), BillPayScheduleVM(billPayScheduleVM));

        return View(billPayScheduleVM);
    }

    [HttpPost]
    public IActionResult SubmitSchedule(BillPayScheduleViewModel billPayScheduleVM)
    {
        List<ValidationResult> errors = _bankService.SubmitBillPay(billPayScheduleVM.AccountNumber.GetValueOrDefault(),
            billPayScheduleVM.PayeeID.GetValueOrDefault(), billPayScheduleVM.Amount.GetValueOrDefault(),
            billPayScheduleVM.ScheduledTimeLocal.GetValueOrDefault(), billPayScheduleVM.Period.GetValueOrDefault());

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Schedule), BillPayScheduleVM(billPayScheduleVM));

        return RedirectToAction(nameof(SuccessfullyScheduled));
    }

    public IActionResult SuccessfullyScheduled()
    {
        List<BillPayViewModel> billPayVM = BillPaysVM();

        (bool displayBlocked, bool displayFailed) = CheckBlockedAndFailed(billPayVM);

        ViewBag.DisplayBlocked = displayBlocked;
        ViewBag.DisplayFailed = displayFailed;
        ViewBag.DisplaySuccess = true;
        ViewBag.DisplayCancelled = false;

        return View("Index", billPayVM);
    }

    public IActionResult ConfirmCancel(int id)
    {
        BillPay billPay = _bankService.GetBillPay(id);

        if (billPay is null ||
            _bankService.GetAccounts(CustomerID).FindIndex(x => x.AccountNumber == billPay.AccountNumber) < 0)
            return RedirectToAction(nameof(Index));

        return View(BillPayVM(billPay));
    }

    [HttpPost]
    public IActionResult SubmitCancel(BillPayViewModel billPayVM)
    {
        ValidationResult error = _bankService.CancelBillPay(billPayVM.BillPayID);

        if (error is not null)
        {
            ModelState.AddModelError(error.MemberNames.First(), error.ErrorMessage);
            return View(nameof(ConfirmCancel), billPayVM);
        }
        return RedirectToAction(nameof(SuccessfullyCancelled));
    }

    public IActionResult SuccessfullyCancelled()
    {
        List<BillPayViewModel> billPayVM = BillPaysVM();

        (bool displayBlocked, bool displayFailed) = CheckBlockedAndFailed(billPayVM);

        ViewBag.DisplayBlocked = displayBlocked;
        ViewBag.DisplayFailed = displayFailed;
        ViewBag.DisplaySuccess = false;
        ViewBag.DisplayCancelled = true;

        return View("Index", billPayVM);
    }

    private static BillPayViewModel BillPayVM(BillPay billPay)
    {
        return new BillPayViewModel()
        {
            BillPayID = billPay.BillPayID,
            AccountNumber = billPay.AccountNumber,
            PayeeID = billPay.PayeeID,
            Amount = billPay.Amount,
            ScheduledTimeLocal = billPay.ScheduledTimeUtc.ToLocalTime(),
            Period = billPay.Period,
            BillPayStatus = billPay.BillPayStatus
        };
    }

    private List<BillPayViewModel> BillPaysVM()
    {
        List<BillPayViewModel> billPayVM = new();
        foreach (BillPay b in _bankService.GetBillPays(CustomerID))
            billPayVM.Add(BillPayVM(b));
        return billPayVM;
    }

    private BillPayScheduleViewModel BillPayScheduleVM(BillPayScheduleViewModel billPayScheduleVM = null)
    {
        List<AccountViewModel> accountVM = new();
        foreach (Account a in _bankService.GetAccounts(CustomerID))
        {
            accountVM.Add(new AccountViewModel
            {
                AccountNumber = a.AccountNumber,
                AccountType = a.AccountType,
                Balance = a.Balance(),
                AvailableBalance = a.AvailableBalance()
            });
        }
        if (billPayScheduleVM is null)
            return new BillPayScheduleViewModel()
            {
                AccountViewModels = accountVM
            };
        else
        {
            billPayScheduleVM.AccountViewModels = accountVM;
            return billPayScheduleVM;
        }
    }

    private static (bool, bool) CheckBlockedAndFailed(List<BillPayViewModel> billPayVM)
    {
        bool displayBlocked = false;
        bool displayFailed = false;

        foreach (BillPayViewModel b in billPayVM)
        {
            if (b.BillPayStatus == BillPayStatus.Failed)
                displayFailed = true;
            else if (b.BillPayStatus == BillPayStatus.Blocked)
                displayBlocked = true;
        }
        return (displayBlocked, displayFailed);
    }
}