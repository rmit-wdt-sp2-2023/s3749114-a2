using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using BankLibrary.Models;
using CustomerApplication.Models;
using CustomerApplication.Filters;
using BankLibrary.Utilities.Paging;
using SimpleHashing.Net;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class DashboardController : Controller
{
    private readonly BankContext _context;

    private static readonly ISimpleHash s_simpleHash = new SimpleHash();

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public DashboardController(BankContext context) => _context = context;

    public IActionResult Index() => View(MakeAccountsViewModel());

    // DEPOSIT

    public IActionResult Deposit() => View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Deposit));

    [HttpPost]
    public IActionResult Deposit(TransactionViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Deposit));
        }
        Account account = _context.Accounts.Find(viewModel.AccountNumber);
        viewModel.AccountType = account.AccountType;
        return RedirectToAction(nameof(Confirm), viewModel);
    }

    // WITHDRAW

    public IActionResult Withdraw() => View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Withdraw));

    [HttpPost]
    public IActionResult Withdraw(TransactionViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Deposit));
        viewModel.AccountType = _context.Accounts.Find(viewModel.AccountNumber).AccountType;
        return RedirectToAction(nameof(Confirm), viewModel);
    }

    // TRANSFER

    public IActionResult Transfer() => View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Transfer));

    [HttpPost]
    public IActionResult Transfer(TransactionViewModel viewModel)
    {
        if (viewModel.DestinationNumber == null)
            ModelState.AddModelError(nameof(viewModel.DestinationNumber), "You must enter an Account Number.");

        if (viewModel.DestinationNumber == viewModel.AccountNumber)
            ModelState.AddModelError(
                nameof(viewModel.DestinationNumber), "To and From Account Numbers cannot be the same.");

        if (!ModelState.IsValid)
            return View(nameof(Transaction), MakeTransactionViewModel(TransactionType.Transfer));

        viewModel.AccountType = _context.Accounts.Find(viewModel.AccountNumber).AccountType;
        return RedirectToAction(nameof(Confirm), viewModel);
    }

    // RESULT

    public TransactionViewModel CommitDeposit(TransactionViewModel viewModel, Account account)
    {
        viewModel.TransactionResult = true;
        Transaction transaction = account.Deposit(viewModel.Amount, viewModel.Comment);
        _context.Transactions.Add(transaction);
        return viewModel;
    }

    public TransactionViewModel CommitWithdraw(TransactionViewModel viewModel, Account account)
    {
        (List<Transaction> transactions, string message) = account.Withdraw(viewModel.Amount, viewModel.Comment);
        if (transactions is null)
        {
            ModelState.AddModelError(nameof(viewModel.TransactionResult), message);
            return viewModel;
        }
        viewModel.TransactionResult = true;
        foreach (Transaction transaction in transactions)
            _context.Transactions.Add(transaction);
        return viewModel;
    }

    public TransactionViewModel CommitTransfer(TransactionViewModel viewModel, Account account)
    {
        Account destinationAccount = _context.Accounts.Find(viewModel.DestinationNumber);
        if (destinationAccount is null)
        {
            ModelState.AddModelError(nameof(viewModel.TransactionResult), "Destination Number doesn't exist.");
            return viewModel;
        }
        (List<Transaction> transactionsFrom, string message) = account.TransferFrom(
            viewModel.DestinationNumber, viewModel.Amount, viewModel.Comment);

        if (transactionsFrom is null)
        {
            ModelState.AddModelError(nameof(viewModel.TransactionResult), message);
            return viewModel;
        }
        viewModel.TransactionResult = true;

        foreach (Transaction transaction in transactionsFrom)
            _context.Transactions.Add(transaction);

        Transaction transactionsTo = destinationAccount.TransferTo(viewModel.Amount, viewModel.Comment);
        _context.Transactions.Add(transactionsTo);

        return viewModel;
    }

    [HttpPost]
    public IActionResult Result(TransactionViewModel viewModel)
    {
        Account account = _context.Accounts.Find(viewModel.AccountNumber);

        if (viewModel.TransactionType == TransactionType.Deposit)
            viewModel = CommitDeposit(viewModel, account);

        else if (viewModel.TransactionType == TransactionType.Withdraw)
            viewModel = CommitWithdraw(viewModel, account);

        else if (viewModel.TransactionType == TransactionType.Transfer)
            viewModel = CommitTransfer(viewModel, account);

        _context.SaveChanges();
        return View(viewModel);
    }

    // CONFIRM

    public IActionResult Confirm(TransactionViewModel viewModel) => View(viewModel);

    // STATEMENTS

    public IActionResult Statements() => View(MakeStatementsViewModel());

    [HttpPost]
    public IActionResult TransactionHistory(int id, StatementsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        Account account = _context.Accounts.Find(viewModel.AccountNumber);
        List<Transaction> transactions = account.Transactions.OrderByDescending(x => x.TransactionTimeUtc).ToList();
        viewModel.PageNumber = id;
        viewModel.TransactionPages = Paging.CalculateTotalPages(transactions.Count, viewModel.PageSize);
        viewModel.TotalPages = viewModel.TransactionPages == 0 ? 1 : viewModel.TransactionPages;
        viewModel.Transactions = Paging.GetPage(transactions, viewModel.PageNumber, viewModel.PageSize);
        viewModel.AccountsViewModel = MakeAccountsViewModel();
        return View(nameof(Statements), viewModel);
    }

    // PROFILE

    public IActionResult Profile() => View(MakeCustomerViewModel());

    public IActionResult EditDetails() => View(MakeCustomerViewModel());

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

        if (viewModel.NewPassword != viewModel.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(viewModel.ConfirmPassword), "Passwords don't match.");
            return View(nameof(ChangePassword), viewModel);
        }

        Login login = _context.Logins.FirstOrDefault(x => x.CustomerID == CustomerID);

        if (login is null)
        {
            ModelState.AddModelError("PasswordFailed", "Password change failed, couldn't find user.");
            return View(nameof(ChangePassword), viewModel);
        }

        if (!s_simpleHash.Verify(viewModel.OldPassword, login.PasswordHash))
        {
            ModelState.AddModelError(nameof(viewModel.OldPassword), "Incorrect password.");
            return View(nameof(ChangePassword), viewModel);
        }

        login.PasswordHash = s_simpleHash.Compute(viewModel.NewPassword);
        _context.Logins.Update(login);
        _context.SaveChanges();
        viewModel.PasswordUpdated = true;

        return View(nameof(ChangePassword), viewModel);
    }

    // VIEW MODEL CREATION

    public CustomerViewModel MakeCustomerViewModel()
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

    public StatementsViewModel MakeStatementsViewModel()
    {
        return new StatementsViewModel
        {
            AccountsViewModel = MakeAccountsViewModel()
        };
    }

    public TransactionViewModel MakeTransactionViewModel(TransactionType transactionType)
    {
        return new TransactionViewModel
        {
            TransactionType = transactionType,
            AccountsViewModel = MakeAccountsViewModel()
        };
    }

    public List<AccountViewModel> MakeAccountsViewModel()
    {
        List<Account> accounts =
            _context.Accounts.Where(x => x.CustomerID == CustomerID).OrderBy(x => x.AccountNumber).ToList();
        List<AccountViewModel> viewModel = new();
        foreach (Account account in accounts)
        {
            viewModel.Add(new AccountViewModel
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                Balance = account.CalculateBalance(),
                AvailableBalance = account.AvailableBalance()
            }); 
        }
        return viewModel;
    }
}
