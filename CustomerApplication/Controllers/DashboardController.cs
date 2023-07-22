using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using CustomerApplication.ViewModels;
using CustomerApplication.Models;
using CustomerApplication.Filters;
using CustomerApplication.Services;
using CustomerApplication.Utilities.Paging;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class DashboardController : Controller
{
    private readonly BankService _bankService;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public DashboardController(BankService bankService) => _bankService = bankService;

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
        List<ValidationResult> errors = _bankService.ConfirmWithdraw(
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
        List<ValidationResult> errors = _bankService.ConfirmDeposit(
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
        List<ValidationResult> errors = _bankService.ConfirmTransfer(
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
        Account account = _bankService.GetAccount(viewModel.AccountNumber);
        viewModel.AccountType = account.AccountType;
        return View(viewModel);
    }

    // Methods for processing confirmed transactions.
    // If data is valid, the database is updated and the user is taken to the success page.
    // If data is invalid, the user is taken back to the submission page. 

    [HttpPost]
    public IActionResult ConfirmDeposit(TransactionViewModel viewModel)
    {
        List<ValidationResult> errors = _bankService.SubmitDeposit(
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
        List<ValidationResult> errors = _bankService.SubmitWithdraw(
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
        List<ValidationResult> errors = _bankService.SubmitTransfer(
                viewModel.AccountNumber, viewModel.DestinationNumber, viewModel.Amount, viewModel.Comment);

        if (errors is null)
            return View(nameof(Success), viewModel);

        foreach (ValidationResult e in errors)
            ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);
  
        return View(nameof(Transaction), TransactionViewModel(TransactionType.Transfer));
    }

    // Displays the transaction success page. 

    public IActionResult Success(TransactionViewModel viewModel) => View(viewModel);

    // Displays the statements page. 

    public IActionResult Statements() => View(StatementsViewModel());

    [HttpPost]
    public IActionResult Statements(int id, StatementsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        List<Transaction> transactions = _bankService.GetTransactions(viewModel.AccountNumber.GetValueOrDefault());

        viewModel.PageNumber = id;
        viewModel.TransactionPages = Paging.CalculateTotalPages(transactions.Count, viewModel.PageSize);
        viewModel.TotalPages = viewModel.TransactionPages == 0 ? 1 : viewModel.TransactionPages;
        viewModel.Transactions = Paging.GetPage(transactions, viewModel.PageNumber, viewModel.PageSize);
        viewModel.AccountsViewModel = AccountsViewModel();

        return View(nameof(Statements), viewModel);
    }

    // Displays the BillPay page.

    public IActionResult BillPay() => View();

    public IActionResult ScheduleBillPay() => View();

    public IActionResult SubmitBillPay() => View();

    // Displays the profile page.

    public IActionResult Profile() => View(CustomerViewModel());

    // Displays the edit profile page.

    public IActionResult EditDetails()
    {
        ViewBag.DisplaySuccess = false;
        return View(CustomerViewModel());
    }

    // Processes edits to the user profile and reloads the page with success or error messages. 

    [HttpPost]
    public IActionResult SubmitEditDetails(CustomerViewModel viewModel)
    {
        List<ValidationResult> errors = _bankService.UpdateCustomer(CustomerID, viewModel.Name, viewModel.TFN,
            viewModel.Address, viewModel.City, viewModel.State, viewModel.PostCode, viewModel.Mobile);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
        {
            ViewBag.DisplaySuccess = false;
            return View(nameof(EditDetails), viewModel);
        }
        ViewBag.DisplaySuccess = true;
        return View(nameof(EditDetails), viewModel);
    }

    // Displays the change password page.

    public IActionResult ChangePassword()
    {
        ViewBag.DisplaySuccess = false;
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    public IActionResult SubmitChangePassword(ChangePasswordViewModel viewModel)
    {
        List<ValidationResult> errors = _bankService.ChangePassword(
            CustomerID, viewModel.OldPassword, viewModel.NewPassword);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);
  
        if (!ModelState.IsValid)
        {
            ViewBag.DisplaySuccess = false;
            return View(nameof(ChangePassword), viewModel);
        }
        ViewBag.DisplaySuccess = true;
        return View(nameof(ChangePassword), viewModel);
    }

    // VIEW MODEL CREATION

    public CustomerViewModel CustomerViewModel()
    {
        Customer customer = _bankService.GetCustomer(CustomerID);
        return new CustomerViewModel
        {
            Name = customer.Name,
            TFN = customer.TFN,
            Address = customer.Address,
            City = customer.City,
            State = customer.State,
            PostCode= customer.PostCode,
            Mobile = customer.Mobile
        };
    }

    public StatementsViewModel StatementsViewModel()
    {
        return new StatementsViewModel
        {
            AccountsViewModel = AccountsViewModel()
        };
    }

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