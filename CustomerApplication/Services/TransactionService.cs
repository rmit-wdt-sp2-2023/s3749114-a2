using CustomerApplication.Data;
using CustomerApplication.Models;
using System.ComponentModel.DataAnnotations;
using X.PagedList;

namespace CustomerApplication.Services;

public class TransactionService
{
    private readonly BankContext _context;

    private readonly AccountService _accountService;

    public TransactionService(BankContext context, AccountService accountService)
    {
        _accountService = accountService;
        _context = context;
    }

    public List<Transaction> GetTransactions(int accountNum)
    {
        Account account = _accountService.GetAccount(accountNum);

        if (account is not null)
        {
            List<Transaction> transactions = account.Transactions.OrderByDescending(x => x.TransactionTimeUtc).ToList();

            if (transactions.Count > 0)
                return transactions;
        }
        return null;
    }

    public IPagedList<Transaction> GetPagedTransactions(int accountNum, int page, int pageSize)
    {
        IPagedList<Transaction> pagedList = _context.Transactions.Where(x => x.AccountNumber == accountNum)
            .OrderByDescending(x => x.TransactionTimeUtc).ToPagedList(page, pageSize);
        return pagedList.Count > 0 ? pagedList : null;
    }




    // Methods validate transactions but don't update database.

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



    // Methods validate transactions and update the database if valid.



    public List<ValidationResult> SubmitDeposit(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, Transaction transaction) = Deposit(accountNum, amount, comment);
        if (errors is null)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
        return errors;
    }

    public List<ValidationResult> SubmitWithdraw(int accountNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, List<Transaction> transactions) = Withdraw(accountNum, amount, comment);
        if (errors is null)
        {
            foreach (Transaction t in transactions)
                _context.Transactions.Add(t);
            _context.SaveChanges();
        }
        return errors;
    }

    public List<ValidationResult> SubmitTransfer(int accountNum, int? destinationNum, decimal amount, string comment)
    {
        (List<ValidationResult> errors, List<Transaction> transactions) =
            Transfer(accountNum, destinationNum, amount, comment);

        if (errors is null)
        {
            foreach (Transaction t in transactions)
                _context.Transactions.Add(t);
            _context.SaveChanges();
        }
        return errors;
    }

    // Methods to help validate transactions but don't update the database.
    // Returns all associated transaction if valid or errors if invalid.

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

    // Methods to help validate transfer transactions but don't update the database.

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