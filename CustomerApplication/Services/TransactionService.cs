using BankLibrary.Data;
using BankLibrary.Models;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace CustomerApplication.Services;

// Service class for deposit, withdraw and transfer transactions.
// Transactions relating to BillPay are located in the BillPayBackgroundService class. 

public class TransactionService
{
    private readonly BankContext _context;

    private readonly AccountService _accountService;

    public TransactionService(BankContext context, AccountService accountService)
    {
        _accountService = accountService;
        _context = context;
    }

    // Takes a page number and page size and returns the transactions on that page.

    public IPagedList<Transaction> GetPagedTransactions(int accountNum, int page, int pageSize)
    {
        IPagedList<Transaction> pagedList = _context.Transactions.Where(x => x.AccountNumber == accountNum)
            .OrderByDescending(x => x.TransactionTimeUtc).ToPagedList(page, pageSize);

        return pagedList.Count > 0 ? pagedList : null;
    }

    // Methods that validate transactions but NOT update database.

    public List<ValidationResult> ConfirmDeposit(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, _) = Deposit(accountNum, amount, comment);
        return errors;
    }

    public List<ValidationResult> ConfirmWithdraw(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, _) = Withdraw(accountNum, amount, comment);
        return errors;
    }

    public List<ValidationResult> ConfirmTransfer(int accountNum, int? destinationNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, _) = Transfer(accountNum, destinationNum, amount, comment);
        return errors;
    }

    // Methods that validate transactions AND update database.

    public (List<ValidationResult>, Transaction) SubmitDeposit(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, Transaction transaction) = Deposit(accountNum, amount, comment);

        if (errors is null)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
        return (errors, transaction);
    }

    public (List<ValidationResult>, List<Transaction>) SubmitWithdraw(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, List<Transaction> transactions) = Withdraw(accountNum, amount, comment);

        if (errors is null)
        {
            foreach (Transaction t in transactions)
                _context.Transactions.Add(t);
            _context.SaveChanges();
        }
        return (errors, transactions);
    }

    public (List<ValidationResult>, List<Transaction>) SubmitTransfer(
        int accountNum, int? destinationNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, List<Transaction> transactions) =
            Transfer(accountNum, destinationNum, amount, comment);

        if (errors is null)
        {
            foreach (Transaction t in transactions)
                _context.Transactions.Add(t);
            _context.SaveChanges();
        }
        return (errors, transactions);
    }

    // Private helper methods that validate transactions but NOT update database.

    private (List<ValidationResult>, Transaction) Deposit(int accountNum, decimal amount, string comment)
    {
        (ValidationResult error, Account account) = _accountService.GetAccount(accountNum, "AccountNumber");
        return error is not null ? (new List<ValidationResult>() { error }, null) : account.Deposit(amount, comment);
    }

    private (List<ValidationResult>, List<Transaction>) Withdraw(int accountNum, decimal amount, string comment)
    {
        (ValidationResult error, Account account) = _accountService.GetAccount(accountNum, "AccountNumber");
        return error is not null ? (new List<ValidationResult>() { error }, null) : account.Withdraw(amount, comment);
    }

    private (List<ValidationResult>, List<Transaction>) Transfer(
        int accountNum, int? destinationNum, decimal amount, string comment)
    {
        List<ValidationResult> errors = new();
        List<Transaction> transactions = new();

        TransferFrom(accountNum, destinationNum, amount, comment, ref errors, ref transactions);
        TransferTo(destinationNum, amount, comment, ref errors, ref transactions);

        return errors.Count > 0 ? (errors, null) : (null, transactions);
    }

    private void TransferTo(int? destinationNum, decimal amount, string comment,
     ref List<ValidationResult> errors, ref List<Transaction> transactions)
    {
        if (destinationNum is null)
            errors.Add(new ValidationResult("Enter an account number.", new List<string>() { "DestinationNumber" }));
        else
        {
            (ValidationResult error, Account destinationAccount) = _accountService.GetAccount(
                destinationNum.GetValueOrDefault(), "DestinationNumber");

            if (error is not null)
                errors.Add(error);
            else
            {
                (_, Transaction transaction) = destinationAccount.TransferTo(amount, comment);
                transactions.Add(transaction);
            }
        }
    }

    private void TransferFrom(int accountNum, int? destinationNum, decimal amount,
        string comment, ref List<ValidationResult> errors, ref List<Transaction> transactions)
    {
        (ValidationResult error, Account account) = _accountService.GetAccount(accountNum, "AccountNumber");

        if (error is not null)
            errors.Add(error);
        else
        {
            (List<ValidationResult> errorsFrom, List<Transaction> transactionsFrom) =
                account.TransferFrom(destinationNum, amount, comment);

            if (errorsFrom is not null)
                errors.AddRange(errorsFrom);
            else
                transactions.AddRange(transactionsFrom);
        }
    }
}