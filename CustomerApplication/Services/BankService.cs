using SimpleHashing.Net;
using CustomerApplication.Data;
using CustomerApplication.Models;

namespace CustomerApplication.Services;

public class BankService
{
    private readonly BankContext _context;

    private static readonly ISimpleHash SimpleHash = new SimpleHash();

    public BankService(BankContext context) => _context = context;

    public Login Login(string loginID, string password)
    {
        Login login = _context.Logins.Find(loginID);
        if (login is not null)
        {
            if (SimpleHash.Verify(password, login.PasswordHash))
                return login;
        }
        return null;
    }

    public List<Account> GetAccounts(int customerID) =>
        _context.Customers.Find(customerID).Accounts.OrderBy(x => x.AccountNumber).ToList();

    public Account GetAccount(int accountNumber) => _context.Accounts.Find(accountNumber);

    public List<Transaction> GetTransactions(int accountNumber)
    {
        Account account = _context.Accounts.Find(accountNumber);

        if (account is null)
            return null;

        List<Transaction> transactions = account.Transactions.OrderByDescending(x => x.TransactionTimeUtc).ToList();

        if (transactions.Count <= 0)
            return null;

        return transactions;
    }

    // Commits a deposit to the database.
    // Returns an error message if unsuccessful. 

    public string Deposit(int accountNumber, decimal amount, string comment)
    {
        Account account = _context.Accounts.Find(accountNumber);

        if (account is null)
            return "Account does not exist.";

        Transaction transaction = account.Deposit(amount, comment);

        _context.Transactions.Add(transaction);
        _context.SaveChanges();

        return null;
    }

    // Commits a withdraw to the database.
    // Returns an error message if unsuccessful. 

    public string Withdraw(int accountNumber, decimal amount, string comment)
    {
        Account account = _context.Accounts.Find(accountNumber);

        if (account is null)
            return "Account does not exist.";

        (List<Transaction> transactions, string errorMessage) = account.Withdraw(amount, comment);

        if (transactions is null)
            return errorMessage;

        foreach (Transaction transaction in transactions)
            _context.Transactions.Add(transaction);
        _context.SaveChanges();

        return null;
    }

    // Commits a transfer to the database.
    // Returns an error message if unsuccessful. 

    public string Transfer(int accountNumber, int? destinationNumber, decimal amount, string comment)
    {
        Account account = _context.Accounts.Find(accountNumber);

        if (account is null)
            return "Origin account does not exist.";

        if (destinationNumber is null)
            return "Must enter a destination number.";

        Account destinationAccount = _context.Accounts.Find(destinationNumber);

        if (destinationAccount is null)
            return "Destination account does not exist.";

        (List<Transaction> transactionsFrom, string message) = account.TransferFrom(destinationNumber, amount, comment);

        if (transactionsFrom is null)
            return message;

        foreach (Transaction transaction in transactionsFrom)
            _context.Transactions.Add(transaction);

        Transaction transactionsTo = destinationAccount.TransferTo(amount, comment);
        _context.Transactions.Add(transactionsTo);

        _context.SaveChanges();

        return null;
    }
}