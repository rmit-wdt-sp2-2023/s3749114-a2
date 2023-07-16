using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Data;
using BankLibrary.Models;
using CustomerApplication.Models;
using CustomerApplication.Filters;
using BankLibrary.Utilities.Paging;
using BankLibrary.Utilities.Miscellaneous;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class DashboardController : Controller
{
    private readonly BankContext _context;

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
    public IActionResult MakeStatement(int id, StatementsViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        Account account = _context.Accounts.Find(viewModel.AccountNumber);

        List<Transaction> transactions = account.Transactions.OrderByDescending(x => x.TransactionTimeUtc).ToList();

 
        viewModel.PageNumber = id;

        viewModel.TransactionPages ??= Paging.CalculateTotalPages(transactions.Count, 4);

        viewModel.TotalPages ??= viewModel.TransactionPages == 0 ? 1 : viewModel.TransactionPages;

        viewModel.Transactions = Paging.GetPage(transactions, viewModel.PageNumber, 4);

        viewModel.AccountsViewModel = MakeAccountsViewModel();

        Console.WriteLine("PAGE NUM IS " + viewModel.PageNumber);
        Console.WriteLine("ID NUM IS " + viewModel.PageNumber);

        return View(nameof(Statements), viewModel);
    }


    // VIEW MODEL CREATION

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
