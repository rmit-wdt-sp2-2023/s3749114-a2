using System.ComponentModel.DataAnnotations;
using BankLibrary.Models;
using Xunit;

namespace BankLibrary.Tests.Models;

public class AccountTests
{
    [Theory]
    [InlineData(4100, AccountType.Checking, 2100)]
    [InlineData(4200, AccountType.Saving, 2200)]
    public void AccountInitialiser_ValidProperties_Success(int accountNumber, AccountType accountType, int customerID)
		{
        Account account = new()
        {
            AccountNumber = accountNumber,
            AccountType = accountType,
            CustomerID = customerID
        };
        Assert.NotNull(account);
        Assert.Equal(accountNumber, account.AccountNumber);
        Assert.Equal(accountType, account.AccountType);
        Assert.Equal(customerID, account.CustomerID);
    }

    [Fact]
    public void Balance_ValidTransactions_Success()
    {
        Account account = new()
        {
            AccountNumber = 4100,
            AccountType = AccountType.Saving,
            CustomerID = 2100,
            Transactions = new() 
            {
                new Transaction()
                {
                    TransactionType = TransactionType.Deposit,
                    Amount = 200,
                    AccountNumber = 4100
                },
                new Transaction()
                {
                    TransactionType = TransactionType.Withdraw,
                    Amount = 2,
                    AccountNumber = 4100
                },
                new Transaction()
                {
                    TransactionType = TransactionType.Transfer,
                    Amount = 10,
                    AccountNumber = 4100,
                    DestinationNumber = 4300
                },
                new Transaction()
                {
                    TransactionType = TransactionType.ServiceCharge,
                    Amount = 0.10M,
                    AccountNumber = 4100,
                },
                new Transaction()
                {
                    TransactionType = TransactionType.Transfer,
                    Amount = 8,
                    AccountNumber = 4100
                },
                new Transaction()
                {
                    TransactionType = TransactionType.BillPay,
                    Amount = 20,
                    AccountNumber = 4100
                }
            }
        };
        Assert.Equal(175.9M, account.Balance());
    }

    [Theory]
    [InlineData(AccountType.Saving, 200)]
    [InlineData(AccountType.Checking, 500)]
    public void AvailableBalance_AvailableBalance_MoreThanZero(AccountType accountType, decimal amount)
    {
        Account account = new()
        {
            AccountNumber = 4100,
            AccountType = accountType,
            CustomerID = 2100,
            Transactions = new()
            {
                new Transaction()
                {
                    TransactionType = TransactionType.Deposit,
                    Amount = amount,
                    AccountNumber = 4100
                }
            }
        };
        Assert.Equal(200, account.AvailableBalance());
    }

    [Theory]
    [InlineData(AccountType.Saving, 0)]
    [InlineData(AccountType.Checking, 300)]
    public void AvailableBalance_NoAvailableBalance_Zero(AccountType accountType, decimal amount)
    {
        Account account = new()
        {
            AccountNumber = 4100,
            AccountType = accountType,
            CustomerID = 2100,
            Transactions = new()
            {
                new Transaction()
                {
                    TransactionType = TransactionType.Deposit,
                    Amount = amount,
                    AccountNumber = 4100
                }
            }
        };
        Assert.Equal(0, account.AvailableBalance());
    }

    [Theory]
    [InlineData(10, null)]
    [InlineData(500, "Testing a deposit")]
    public void Deposit_ValidParameters_Success(decimal amount, string comment)
    {
        Account account = new()
        {
            AccountNumber = 4100,
            AccountType = AccountType.Saving,
            CustomerID = 2100,
        };

        (List<ValidationResult> errors, Transaction transaction) = account.Deposit(amount, comment);

        Assert.NotNull(transaction);
        Assert.Null(errors);
        Assert.Equal(amount, account.AvailableBalance());
        Assert.Equal(transaction, account.Transactions.First());
    }

    [Theory]
    [InlineData(-10, null)]
    [InlineData(10.5555, null)]
    [InlineData(5, "This is a test comment with more than 30 characters")]
    public void Deposit_InvalidParameters_ErrorMessages(decimal amount, string comment)
    {
        Account account = new()
        {
            AccountNumber = 4100,
            AccountType = AccountType.Saving,
            CustomerID = 2100,
        };
    
        (List<ValidationResult> errors, Transaction transaction) = account.Deposit(amount, comment);

        Assert.Null(transaction);
        Assert.NotNull(errors);
    }

    [Theory]
    [InlineData(10, null)]
    [InlineData(10, "Testing a withdraw")]
    public void Withdraw_ValidParameters_Success(decimal amount, string comment)
    {
        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, List<Transaction> transactions) = account.Withdraw(amount, comment);

        Assert.NotNull(transactions);
        Assert.Null(errors);
        Assert.Equal(390, account.AvailableBalance());
        Assert.Contains(transactions.First(), account.Transactions);
    }

    [Theory]
    [InlineData(-10, null)]
    [InlineData(600.77777, null)]
    [InlineData(10, "This is a test comment with more than 30 characters")]
    public void Withdraw_InvalidParameters_ErrorMessages(decimal amount, string comment)
    {
        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, List<Transaction> transactions) = account.Withdraw(amount, comment);

        Assert.NotNull(errors);
        Assert.Null(transactions);
    }

    [Theory]
    [InlineData(401, null, AccountType.Saving)]
    [InlineData(101, null, AccountType.Checking)]
    public void Withdraw_MoreThanMinAmount_ErrorMessages(
        decimal amount, string comment, AccountType accountType)
    {
        Account account = CreateAccountFourHundredBalance(accountType);

        (List<ValidationResult> errors, List<Transaction> transactions) = account.Withdraw(amount, comment);

        Assert.NotNull(errors);
        Assert.Null(transactions);
    }

    [Theory]
    [InlineData(2244, 10, null, 390)]
    [InlineData(4300, 50, "Sending a transfer", 350)]
    public void TransferFrom_ValidParameters_Success(
        int? destinationNum, decimal amount, string comment, decimal expectedBalance)
    {
        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, List<Transaction> transactions) =
            account.TransferFrom(destinationNum, amount, comment);

        Assert.NotNull(transactions);
        Assert.Null(errors);
        Assert.Equal(expectedBalance, account.AvailableBalance());
        Assert.Contains(transactions.First(), account.Transactions);
    }

    [Theory]
    [InlineData(null, 10, null)]
    [InlineData(4300, -10, null)]
    [InlineData(4300, 10.555, null)]
    [InlineData(1234567, 10, null)]
    [InlineData(4300, 10, "This is a test comment with more than 30 characters")]
    public void TransferFrom_InvalidParameters_ErrorMessages(int? destinationNum, decimal amount, string comment)
    {
        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, List<Transaction> transactions) =
            account.TransferFrom(destinationNum, amount, comment);

        Assert.NotNull(errors);
        Assert.Null(transactions);
    }

    [Theory]
    [InlineData(4000, 401, null, AccountType.Saving)]
    [InlineData(4000, 101, null, AccountType.Checking)]
    public void TransferFrom_MoreThanMinAmount_ErrorMessages(
        int? destinationNum, decimal amount, string comment, AccountType accountType)
    {
        Account account = CreateAccountFourHundredBalance(accountType);

        (List<ValidationResult> errors, List<Transaction> transactions) =
            account.TransferFrom(destinationNum, amount, comment);

        Assert.NotNull(errors);
        Assert.Null(transactions);
    }

    [Theory]
    [InlineData(10, null, 410)]
    [InlineData(50, "Sending a transfer", 450)]
    public void TransferTo_ValidParameters_Success(decimal amount, string comment, decimal expectedBalance)
    {
        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, Transaction transaction) = account.TransferTo(amount, comment);

        Assert.NotNull(transaction);
        Assert.Null(errors);
        Assert.Equal(expectedBalance, account.AvailableBalance());
        Assert.Contains(transaction, account.Transactions);
    }

    [Theory]
    [InlineData(-10, null)]
    [InlineData(10.5555, null)]
    [InlineData(10, "This is a test comment with more than 30 characters")]
    public void TransferTo_InvalidParameters_ErrorMessages(decimal amount, string comment)
    {
        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, Transaction transaction) = account.TransferTo(amount, comment);

        Assert.NotNull(errors);
        Assert.Null(transaction);
    }

    [Theory]
    [InlineData(1, 10, Period.Monthly)]
    [InlineData(2, 550, Period.OneOff)]
    public void BillPaySchedule_ValidParameters_Success(int payeeID, decimal amount, Period period)
    {
        DateTime scheduledTimeLocal = DateTime.Now.AddMinutes(11);

        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, BillPay billPay) =
            account.BillPaySchedule(payeeID, amount, scheduledTimeLocal, period);

        Assert.NotNull(billPay);
        Assert.Null(errors);
        Assert.Contains(billPay, account.BillPays);
    }

    [Theory]
    [InlineData(1, -10, Period.Monthly)]
    [InlineData(2, 55.5555, Period.OneOff)]
    public void BillPaySchedule_InvalidParameters_ErrorMessages(int payeeID, decimal amount, Period period)
    {
        DateTime scheduledTimeLocal = DateTime.Now.AddMinutes(11);

        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, BillPay billPay) =
            account.BillPaySchedule(payeeID, amount, scheduledTimeLocal, period);

        Assert.NotNull(errors);
        Assert.Null(billPay);
    }

    [Fact]
    public void BillPaySchedule_TimeLessThan10MinsInFuture_ErrorMessages()
    {
        DateTime scheduledTimeLocal = DateTime.Now.AddMinutes(9);

        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, BillPay billPay) =
            account.BillPaySchedule(1, 10, scheduledTimeLocal, Period.Monthly);

        Assert.NotNull(errors);
        Assert.Null(billPay);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(400)]
    public void BillPay_ValidParameters_Success(decimal amount)
    {
        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, Transaction transaction) = account.BillPay(amount);

        Assert.NotNull(transaction);
        Assert.Null(errors);
        Assert.Contains(transaction, account.Transactions);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(10.5555)]
    public void BillPay_InvalidParameters_ErrorMessages(decimal amount)
    {
        Account account = CreateAccountFourHundredBalance();

        (List<ValidationResult> errors, Transaction transaction) = account.BillPay(amount);

        Assert.NotNull(errors);
        Assert.Null(transaction);
    }

    [Theory]
    [InlineData(401, AccountType.Saving)]
    [InlineData(101, AccountType.Checking)]
    public void BillPay_MoreThanMinAmount_ErrorMessages(decimal amount, AccountType accountType)
    {
        Account account = CreateAccountFourHundredBalance(accountType);

        (List<ValidationResult> errors, Transaction transaction) = account.BillPay(amount);

        Assert.NotNull(errors);
        Assert.Null(transaction);
    }

    // Account creation helper

    private static Account CreateAccountFourHundredBalance(AccountType accountType = AccountType.Saving)
    {
        Account account = new()
        {
            AccountNumber = 4100,
            AccountType = accountType,
            CustomerID = 2100,
        };
        account.Deposit(400, null);
        return account;
    }
}