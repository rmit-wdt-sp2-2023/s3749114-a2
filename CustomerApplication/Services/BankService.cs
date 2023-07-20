using SimpleHashing.Net;
using CustomerApplication.Data;
using CustomerApplication.Models;
using CustomerApplication.Validation;
using System.ComponentModel.DataAnnotations;

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

    public Account GetAccount(int accountNum) => _context.Accounts.Find(accountNum);

    public List<Transaction> GetTransactions(int accountNum)
    {
        Account account = _context.Accounts.Find(accountNum);
        if (account is not null)
        {
            List<Transaction> transactions = account.Transactions.OrderByDescending(x => x.TransactionTimeUtc).ToList();
            if (transactions.Count > 0)
                return transactions;
        }
        return null;
    }

    public Customer GetCustomer(int customerID) => _context.Customers.FirstOrDefault(c => c.CustomerID == customerID);

    private (ValidationResult, Account) GetAccount(int accountNum, string propertyName)
    {
        Account account = _context.Accounts.Find(accountNum);
        if (account is null)
            return (new ValidationResult("Account doesn't exist.", new List<string>() { propertyName }), null);
        return (null, account);
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
        (ValidationResult error, Account account) = GetAccount(accountNum, "AccountNumber");
        return error is not null ? (new List<ValidationResult>() { error }, null) : account.Deposit(amount, comment);
    }

    private (List<ValidationResult>, List<Transaction>) Withdraw(int accountNum, decimal amount, string comment)
    {
        (ValidationResult error, Account account) = GetAccount(accountNum, "AccountNumber");
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
            (ValidationResult error, Account destinationAccount) = GetAccount(
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
        (ValidationResult error, Account account) = GetAccount(accountNum, "AccountNumber");

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

    public List<ValidationResult> UpdateCustomer(int customerID, string name, string TFN,
        string address, string city, string state, string postCode, string mobile)
    {
        Customer customer = _context.Customers.FirstOrDefault(c => c.CustomerID == customerID);
        List<ValidationResult> errors = new();

        if (customer is null)
        {
            errors.Add(new ValidationResult("Could not update. Can't find customer.", new List<string>() { "Other" }));
            return errors;
        }
        customer.Name = name;
        customer.TFN = TFN;
        customer.Address = address;
        customer.City = city;
        customer.State = state.ToUpper();
        customer.PostCode = postCode;
        customer.Mobile = mobile;

        if (!ValidationMethods.Validate(customer, out errors))
            return errors;

        _context.Customers.Update(customer);
        _context.SaveChanges();
        return null;
    }

    public List<ValidationResult> ChangePassword(int customerID, string oldPass, string newPass)
    {
        List<ValidationResult> errors = new();

        if (oldPass is null)
            errors.Add(new ValidationResult("Enter your old password.", new List<string>() { "OldPassword" }));
        if (newPass is null)
            errors.Add(new ValidationResult("Enter a new password.", new List<string>() { "NewPassword" }));
        if (errors.Count > 0)
            return errors;

        Login login = _context.Logins.FirstOrDefault(c => c.CustomerID == customerID);

        if (login is null)
            errors.Add(new ValidationResult("Error, couldn't find user.", new List<string>() { "PasswordFailed" }));
        else
        {
            if (!SimpleHash.Verify(oldPass, login.PasswordHash))
                errors.Add(new ValidationResult("Incorrect password.", new List<string>() { "OldPassword" }));
        }
        if (errors.Count > 0)
            return errors;

        login.PasswordHash = SimpleHash.Compute(newPass);
        _context.Logins.Update(login);
        _context.SaveChanges();
        return null;
    }

    public List<BillPay> GetBillPays(int customerID)
    {
        List<BillPay> billPays = new();

        foreach (Account a in GetAccounts(customerID))
            billPays.AddRange(_context.BillPays.Where(x => x.AccountNumber == a.AccountNumber).ToList());

        return billPays;
    }
        



}