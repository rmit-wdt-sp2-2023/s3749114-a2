using System.ComponentModel.DataAnnotations;
using CustomerApplication.Filters;
using CustomerApplication.Models;
using CustomerApplication.Services;
using CustomerApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Mappers;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class BillPayController : Controller
{
    private readonly BillPayService _billPayService;

    private readonly AccountService _accountService;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public BillPayController(BillPayService billPayService, AccountService accountService)
    {
        _accountService = accountService;
        _billPayService = billPayService;
    }

    public IActionResult Index()
    {
        List<BillPayViewModel> billPayVM = ViewModelMapper.BillPays(_billPayService.GetBillPays(CustomerID));

        (bool displayBlocked, bool displayFailed) = CheckBlockedAndFailed(billPayVM);

        ViewBag.DisplayBlocked = displayBlocked;
        ViewBag.DisplayFailed = displayFailed;
        ViewBag.DisplaySuccess = false;
        ViewBag.DisplayCancelled = false;

        return View(billPayVM);
    }

    public IActionResult Schedule() => View(ViewModelMapper.BillPaySchedule(_accountService.GetAccounts(CustomerID)));

    [HttpPost]
    public IActionResult ConfirmSchedule(BillPayScheduleViewModel billPayScheduleVM)
    {
        List<ValidationResult> errors = _billPayService.ConfirmBillPay(
            billPayScheduleVM.AccountNumber.GetValueOrDefault(), billPayScheduleVM.PayeeID.GetValueOrDefault(),
            billPayScheduleVM.Amount.GetValueOrDefault(), billPayScheduleVM.ScheduledTimeLocal.GetValueOrDefault(),
            billPayScheduleVM.Period.GetValueOrDefault());

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Schedule), ViewModelMapper.BillPaySchedule(
                _accountService.GetAccounts(CustomerID), billPayScheduleVM));

        return View(billPayScheduleVM);
    }

    [HttpPost]
    public IActionResult SubmitSchedule(BillPayScheduleViewModel billPayScheduleVM)
    {
        List<ValidationResult> errors = _billPayService.SubmitBillPay(
            billPayScheduleVM.AccountNumber.GetValueOrDefault(), billPayScheduleVM.PayeeID.GetValueOrDefault(),
            billPayScheduleVM.Amount.GetValueOrDefault(), billPayScheduleVM.ScheduledTimeLocal.GetValueOrDefault(),
            billPayScheduleVM.Period.GetValueOrDefault());

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);
                           
        if (!ModelState.IsValid)
            return View(nameof(Schedule), ViewModelMapper.BillPaySchedule(
                _accountService.GetAccounts(CustomerID), billPayScheduleVM));

        return RedirectToAction(nameof(SuccessfullyScheduled));
    }

    public IActionResult SuccessfullyScheduled()
    {
        List<BillPayViewModel> billPayVM = ViewModelMapper.BillPays(_billPayService.GetBillPays(CustomerID));

        (bool displayBlocked, bool displayFailed) = CheckBlockedAndFailed(billPayVM);

        ViewBag.DisplayBlocked = displayBlocked;
        ViewBag.DisplayFailed = displayFailed;
        ViewBag.DisplaySuccess = true;
        ViewBag.DisplayCancelled = false;

        return View("Index", billPayVM);
    }

    public IActionResult ConfirmCancel(int id)
    {
        BillPay billPay = _billPayService.GetBillPay(id);

        // ID passes through URL, so check that BillPay exists
        // and confirm if it belongs to logged in customer.

        if (billPay is null ||
            _accountService.GetAccounts(CustomerID).FindIndex(x => x.AccountNumber == billPay.AccountNumber) < 0)
            return RedirectToAction(nameof(Index));

        return View(ViewModelMapper.BillPay(billPay));
    }

    [HttpPost]
    public IActionResult SubmitCancel(BillPayViewModel billPayVM)
    {
        ValidationResult error = _billPayService.CancelBillPay(billPayVM.BillPayID);

        if (error is not null)
        {
            ModelState.AddModelError(error.MemberNames.First(), error.ErrorMessage);
            return View(nameof(ConfirmCancel), billPayVM);
        }
        return RedirectToAction(nameof(SuccessfullyCancelled));
    }

    public IActionResult SuccessfullyCancelled()
    {
        List<BillPayViewModel> billPayVM = ViewModelMapper.BillPays(_billPayService.GetBillPays(CustomerID));

        (bool displayBlocked, bool displayFailed) = CheckBlockedAndFailed(billPayVM);

        ViewBag.DisplayBlocked = displayBlocked;
        ViewBag.DisplayFailed = displayFailed;
        ViewBag.DisplaySuccess = false;
        ViewBag.DisplayCancelled = true;

        return View("Index", billPayVM);
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