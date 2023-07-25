using Microsoft.AspNetCore.Mvc;
using CustomerApplication.ViewModels;
using CustomerApplication.Models;
using CustomerApplication.Filters;
using CustomerApplication.Services;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class DashboardController : Controller
{
    private readonly TransactionService _transactionService;

    private readonly AccountService _accountService;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public DashboardController(TransactionService transactionService, AccountService accountService)
    {
        _accountService = accountService;
        _transactionService = transactionService;
    }

    public IActionResult Index() => View(AccountsViewModel());

    // The following methods display transaction submission pages. 

    public IActionResult Withdraw() => View(nameof(Transaction), TransactionViewModel(TransactionType.Withdraw));

    public IActionResult Deposit() => View(nameof(Transaction), TransactionViewModel(TransactionType.Deposit));

    public IActionResult Transfer() => View(nameof(Transaction), TransactionViewModel(TransactionType.Transfer));

    // The following methods process form data for transaction submissions. 
    // If data is valid, the user is taken to the confirmation page.
    // If data is invalid, the user is taken back to the submission page. 

    [HttpPost]
    public IActionResult SubmitWithdraw(TransactionViewModel viewModel)
    {
        List<ValidationResult> errors = _transactionService.ConfirmWithdraw(
            viewModel.AccountNumber, viewModel.Amount, viewModel.Comment);
  
        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Transaction), TransactionViewModel(TransactionType.Withdraw));

        return RedirectToAction(nameof(Confirm), viewModel);
    }

    [HttpPost]
    public IActionResult SubmitDeposit(TransactionViewModel viewModel)
    {
        List<ValidationResult> errors = _transactionService.ConfirmDeposit(
            viewModel.AccountNumber, viewModel.Amount, viewModel.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Transaction), TransactionViewModel(TransactionType.Deposit));

        return RedirectToAction(nameof(Confirm), viewModel);
    }

    [HttpPost]
    public IActionResult SubmitTransfer(TransactionViewModel viewModel)
    {
        List<ValidationResult> errors = _transactionService.ConfirmTransfer(
            viewModel.AccountNumber, viewModel.DestinationNumber, viewModel.Amount, viewModel.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Transaction), TransactionViewModel(TransactionType.Transfer));

        return RedirectToAction(nameof(Confirm), viewModel);
    }

    // Displays the transaction confirmation page.

    public IActionResult Confirm(TransactionViewModel viewModel)
    {
        Account account = _accountService.GetAccount(viewModel.AccountNumber);
        viewModel.AccountType = account.AccountType;
        return View(viewModel);
    }

    // Methods for processing confirmed transactions.
    // If data is valid, the database is updated and the user is taken to the success page.
    // If data is invalid, the user is taken back to the submission page. 

    [HttpPost]
    public IActionResult ConfirmDeposit(TransactionViewModel viewModel)
    {
        List<ValidationResult> errors = _transactionService.SubmitDeposit(
            viewModel.AccountNumber, viewModel.Amount, viewModel.Comment);

        if (errors is null)
            return View(nameof(Success), viewModel);

        foreach (ValidationResult e in errors )
            ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        return View(nameof(Transaction), TransactionViewModel(TransactionType.Deposit));                  
    }

    [HttpPost]
    public IActionResult ConfirmWithdraw(TransactionViewModel viewModel)
    {
        List<ValidationResult> errors = _transactionService.SubmitWithdraw(
            viewModel.AccountNumber, viewModel.Amount, viewModel.Comment);

        if (errors is null)
            return View(nameof(Success), viewModel);

        foreach (ValidationResult e in errors)
            ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        return View(nameof(Transaction), TransactionViewModel(TransactionType.Withdraw));
    }

    [HttpPost]
    public IActionResult ConfirmTransfer(TransactionViewModel viewModel)
    {
        List<ValidationResult> errors = _transactionService.SubmitTransfer(
                viewModel.AccountNumber, viewModel.DestinationNumber, viewModel.Amount, viewModel.Comment);

        if (errors is null)
            return View(nameof(Success), viewModel);

        foreach (ValidationResult e in errors)
            ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);
  
        return View(nameof(Transaction), TransactionViewModel(TransactionType.Transfer));
    }

    // Displays the transaction success page. 

    public IActionResult Success(TransactionViewModel viewModel) => View(viewModel);

    public TransactionViewModel TransactionViewModel(TransactionType transactionType)
    {
        return new TransactionViewModel
        {
            TransactionType = transactionType,
            AccountsViewModel = AccountsViewModel()
        };
    }

    public List<AccountViewModel> AccountsViewModel()
    {
        List<Account> accounts = _accountService.GetAccounts(CustomerID);
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