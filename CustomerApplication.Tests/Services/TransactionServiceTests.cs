using System.ComponentModel.DataAnnotations;
using BankLibrary.Data;
using BankLibrary.Models;
using CustomerApplication.Data;
using CustomerApplication.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CustomerApplication.Tests.Services;

public class TransactionServiceTests : IDisposable
{
    private readonly BankContext _context;

    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
	{
        _context = new BankContext(new DbContextOptionsBuilder<BankContext>()
            .UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);

        _context.Database.EnsureCreated();

        SeedData.Initialise(_context);

        AccountService _accountService = new(_context);

        _transactionService = new TransactionService(_context, _accountService);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Theory]
    [InlineData(4100, 10, "This is a test comment")]
    [InlineData(4100, 5.55, null)]
    public void ConfirmDeposit_ValidParameters_ReturnsNull(int accountNum, decimal amount, string comment)
    {
        List<ValidationResult> errors = _transactionService.ConfirmDeposit(accountNum, amount, comment);

        Assert.Null(errors);
    }

    [Theory]
    [InlineData(41009999, 10, "This is a test comment")]
    [InlineData(4100, 5.55666, "This is a test comment")]
    [InlineData(4100, -10, "This is a test comment")]
    [InlineData(4100, -10, "This is a test comment that is more that 50 characters")]
    public void ConfirmDeposit_InvalidParameters_ReturnsErrors(int accountNum, decimal amount, string comment)
    {
        List<ValidationResult> errors = _transactionService.ConfirmDeposit(accountNum, amount, comment);

        Assert.NotNull(errors);
    }

    [Theory]
    [InlineData(4100, 10, "This is a test comment")]
    [InlineData(4100, 5.55, null)]
    public void ConfirmWithdraw_ValidParameters_ReturnsNull(int accountNum, decimal amount, string comment)
    {
        List<ValidationResult> errors = _transactionService.ConfirmWithdraw(accountNum, amount, comment);
        Assert.Null(errors);
    }

    [Theory]
    [InlineData(41009999, 10, "This is a test comment")]
    [InlineData(4100, 5.55666, "This is a test comment")]
    [InlineData(4100, 1000000000, "More than balance")]
    [InlineData(4100, -10, "This is a test comment")]
    [InlineData(4100, 10, "This is a test comment that is more that 50 characters")]
    public void ConfirmWithdraw_InvalidParameters_ReturnsErrors(int accountNum, decimal amount, string comment)
    {
        List<ValidationResult> errors = _transactionService.ConfirmWithdraw(accountNum, amount, comment);
        Assert.NotNull(errors);
    }

    [Theory]
    [InlineData(4100, 4200, 10, "This is a test comment")]
    [InlineData(4100, 4200, 5.55, null)]
    public void ConfirmTransfer_ValidParameters_ReturnsNull(
        int accountNum, int destinationNum, decimal amount, string comment)
    {
        List<ValidationResult> errors =
            _transactionService.ConfirmTransfer(accountNum, destinationNum, amount, comment);

        Assert.Null(errors);
    }

    [Theory]
    [InlineData(41009999, 4200, 10, "This is a test comment")]
    [InlineData(4100, null, 10, "This is a test comment")]
    [InlineData(4100, 6000, 10, "This is a test comment")]
    [InlineData(4100, 4200, 5.55666, "This is a test comment")]
    [InlineData(4100, 4200, 1000000000, "More than balance")]
    [InlineData(4100, 4200, - 10, "This is a test comment")]
    [InlineData(4100, 4200, 10, "This is a test comment that is more that 50 characters")]
    public void ConfirmTransfer_InvalidParameters_ReturnsErrors(
        int accountNum, int? destinationNum, decimal amount, string comment)
    {
        List<ValidationResult> errors =
            _transactionService.ConfirmTransfer(accountNum, destinationNum, amount, comment);

        Assert.NotNull(errors);
    }

    [Theory]
    [InlineData(4100, 10, "This is a test comment")]
    [InlineData(4100, 5.55, null)]
    public void SubmitDeposit_ValidParameters_UpdatesDatabase(int accountNum, decimal amount, string comment)
    {
        int count = _context.Transactions.Count();

        (List<ValidationResult> errors, Transaction transaction) =
            _transactionService.SubmitDeposit(accountNum, amount, comment);

        Assert.Null(errors);
        Assert.NotNull(transaction);
        Assert.True(_context.Transactions.Count() == count + 1);
    }

    [Theory]
    [InlineData(41009999, 10, "This is a test comment")]
    [InlineData(4100, 5.55666, "This is a test comment")]
    [InlineData(4100, -10, "This is a test comment")]
    [InlineData(4100, -10, "This is a test comment that is more that 50 characters")]
    public void SubmitDeposit_InvalidParameters_DoesntUpdateDatabase(int accountNum, decimal amount, string comment)
    {
        int count = _context.Transactions.Count();

        (List<ValidationResult> errors, Transaction transaction) =
            _transactionService.SubmitDeposit(accountNum, amount, comment);

        Assert.NotNull(errors);
        Assert.Null(transaction);
        Assert.True(_context.Transactions.Count() == count);
    }

    [Theory]
    [InlineData(4100, 10, "This is a test comment")]
    [InlineData(4100, 5.55, null)]
    public void SubmitWithdraw_ValidParameters_UpdatesDatabase(int accountNum, decimal amount, string comment)
    {
        int count = _context.Transactions.Count();

        (List<ValidationResult> errors, List<Transaction> transactions) =
            _transactionService.SubmitWithdraw(accountNum, amount, comment);

        Assert.Null(errors);
        Assert.NotNull(transactions);
        Assert.True(_context.Transactions.Count() == count + 1);
    }

    [Theory]
    [InlineData(41009999, 10, "This is a test comment")]
    [InlineData(4100, 5.55666, "This is a test comment")]
    [InlineData(4100, 1000000000, "More than balance")]
    [InlineData(4100, -10, "This is a test comment")]
    [InlineData(4100, 10, "This is a test comment that is more that 50 characters")]
    public void SubmitWithdraw_InvalidParameters_DoesntUpdateDatabase(int accountNum, decimal amount, string comment)
    {
        int count = _context.Transactions.Count();

        (List<ValidationResult> errors, List<Transaction> transactions) =
            _transactionService.SubmitWithdraw(accountNum, amount, comment);

        Assert.NotNull(errors);
        Assert.Null(transactions);
        Assert.True(_context.Transactions.Count() == count);
    }

    [Theory]
    [InlineData(4100, 4200, 10, "This is a test comment")]
    [InlineData(4100, 4200, 5.55, null)]
    public void SubmitTransfer_ValidParameters_ReturnsNull(
        int accountNum, int destinationNum, decimal amount, string comment)
    {
        int count = _context.Transactions.Count();

        (List<ValidationResult> errors, List<Transaction> transactions) =
            _transactionService.SubmitTransfer(accountNum, destinationNum, amount, comment);

        Assert.Null(errors);
        Assert.NotNull(transactions);
        Assert.True(_context.Transactions.Count() == count + 2);
    }

    [Theory]
    [InlineData(41009999, 4200, 10, "This is a test comment")]
    [InlineData(4100, null, 10, "This is a test comment")]
    [InlineData(4100, 6000, 10, "This is a test comment")]
    [InlineData(4100, 4200, 5.55666, "This is a test comment")]
    [InlineData(4100, 4200, 1000000000, "More than balance")]
    [InlineData(4100, 4200, -10, "This is a test comment")]
    [InlineData(4100, 4200, 10, "This is a test comment that is more that 50 characters")]
    public void SubmitTransfer_InvalidParameters_DoesntUpdateDatabase(
        int accountNum, int? destinationNum, decimal amount, string comment)
    {
        int count = _context.Transactions.Count();

        (List<ValidationResult> errors, List<Transaction> transactions) =
            _transactionService.SubmitTransfer(accountNum, destinationNum, amount, comment);

        Assert.NotNull(errors);
        Assert.Null(transactions);
        Assert.True(_context.Transactions.Count() == count);
    }
}