using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using CustomerApplication.ViewModels;
using CustomerApplication.Models;
using CustomerApplication.Filters;
using CustomerApplication.Services;
using CustomerApplication.Utilities.Paging;
using SimpleHashing.Net;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class DashboardController : Controller
{
    private readonly BankContext _context;

    private readonly BankService _bankService;

    private static readonly ISimpleHash SimpleHash = new SimpleHash();

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public DashboardController(BankContext context, BankService bankService)
    {
        _context = context;
        _bankService = bankService;
    }

    public IActionResult Index() => View(AccountsViewModel());

    public IActionResult Deposit() => View(nameof(Transaction), TransactionViewModel(TransactionType.Deposit));

    public IActionResult Withdraw() => View(nameof(Transaction), TransactionViewModel(TransactionType.Withdraw));

    public IActionResult Transfer() => View(nameof(Transaction), TransactionViewModel(TransactionType.Transfer));

    [HttpPost]
    public IActionResult SubmitDeposit(TransactionViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(nameof(Transaction), TransactionViewModel(TransactionType.Deposit));

        Account account = _bankService.GetAccount(viewModel.AccountNumber);
        viewModel.AccountType = account.AccountType;

        return RedirectToAction(nameof(Confirm), viewModel);
    }

    [HttpPost]
    public IActionResult SubmitWithdraw(TransactionViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(nameof(Transaction), TransactionViewModel(TransactionType.Withdraw));

        Account account = _bankService.GetAccount(viewModel.AccountNumber);
        viewModel.AccountType = account.AccountType;

        return RedirectToAction(nameof(Confirm), viewModel);
    }

    [HttpPost]
    public IActionResult SubmitTransfer(TransactionViewModel viewModel)
    {
        if (viewModel.DestinationNumber == null)
            ModelState.AddModelError(nameof(viewModel.DestinationNumber), "You must enter an account number.");

        if (viewModel.DestinationNumber == viewModel.AccountNumber)
            ModelState.AddModelError(
                nameof(viewModel.DestinationNumber), "To and From Account Numbers cannot be the same.");

        if (!ModelState.IsValid)
            return View(nameof(Transaction), TransactionViewModel(TransactionType.Transfer));

        Account account = _bankService.GetAccount(viewModel.AccountNumber);
        viewModel.AccountType = account.AccountType;

        return RedirectToAction(nameof(Confirm), viewModel);
    }

    public IActionResult Confirm(TransactionViewModel viewModel) => View(viewModel);

    [HttpPost]
    public IActionResult ConfirmDeposit(TransactionViewModel viewModel)
    {
        string errorMessage = _bankService.Deposit(viewModel.AccountNumber, viewModel.Amount, viewModel.Comment);

        if (errorMessage is null)
            viewModel.TransactionResult = true;
        else
            ModelState.AddModelError(nameof(viewModel.TransactionResult), errorMessage);

        return View(nameof(Result), viewModel);
    }

    [HttpPost]
    public IActionResult ConfirmWithdraw(TransactionViewModel viewModel)
    {
        string errorMessage = _bankService.Withdraw(viewModel.AccountNumber, viewModel.Amount, viewModel.Comment);

        if (errorMessage is null)
            viewModel.TransactionResult = true;
        else
            ModelState.AddModelError(nameof(viewModel.TransactionResult), errorMessage);

        return View(nameof(Result), viewModel);
    }

    [HttpPost]
    public IActionResult ConfirmTransfer(TransactionViewModel viewModel)
    {
        string errorMessage = _bankService.Transfer(
            viewModel.AccountNumber, viewModel.DestinationNumber, viewModel.Amount, viewModel.Comment);

        if (errorMessage is null)
            viewModel.TransactionResult = true;
        else
            ModelState.AddModelError(nameof(viewModel.TransactionResult), errorMessage);

        return View(nameof(Result), viewModel);
    }

    public IActionResult Result(TransactionViewModel viewModel) => View(viewModel);

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

    // PROFILE

    public IActionResult Profile() => View(CustomerViewModel());

    public IActionResult EditDetails() => View(CustomerViewModel());

    [HttpPost]
    public IActionResult SubmitEditDetails(CustomerViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(nameof(EditDetails), viewModel);

        Customer customer = _context.Customers.FirstOrDefault(x => x.CustomerID == CustomerID);

        customer.Name = viewModel.Name;
        customer.TFN = viewModel.TFN;
        customer.Address = viewModel.Address;
        customer.City = viewModel.City;
        customer.State = viewModel.State;
        customer.PostCode = viewModel.PostCode;
        customer.Mobile = viewModel.Mobile;

        _context.Customers.Update(customer);
        _context.SaveChanges();
        viewModel.DetailsUpdated = true;

        return View(nameof(EditDetails), viewModel); 
    }

    // PASSWORD

    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    public IActionResult SubmitChangePassword(ChangePasswordViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(nameof(ChangePassword), viewModel);

        Login login = _context.Logins.FirstOrDefault(x => x.CustomerID == CustomerID);

        if (login is null)
        {
            ModelState.AddModelError("PasswordFailed", "Password change failed, couldn't find user.");
            return View(nameof(ChangePassword), viewModel);
        }

        if (!SimpleHash.Verify(viewModel.OldPassword, login.PasswordHash))
        {
            ModelState.AddModelError(nameof(viewModel.OldPassword), "Incorrect password.");
            return View(nameof(ChangePassword), viewModel);
        }

        login.PasswordHash = SimpleHash.Compute(viewModel.NewPassword);
        _context.Logins.Update(login);
        _context.SaveChanges();
        viewModel.PasswordUpdated = true;

        return View(nameof(ChangePassword), viewModel);
    }

    // VIEW MODEL CREATION

    public CustomerViewModel CustomerViewModel()
    {
        Customer customer = _context.Customers.FirstOrDefault(x => x.CustomerID == CustomerID);

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
