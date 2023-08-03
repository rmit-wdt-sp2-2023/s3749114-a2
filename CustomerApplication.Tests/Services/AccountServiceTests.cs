using BankLibrary.Data;
using BankLibrary.Models;
using CustomerApplication.Data;
using CustomerApplication.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Tests.Services;

public class AccountServiceTests : IDisposable
{
    private readonly BankContext _context;

    private readonly AccountService _accountService;

    public AccountServiceTests()
	{
        _context = new BankContext(new DbContextOptionsBuilder<BankContext>()
            .UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);

        _context.Database.EnsureCreated();

        SeedData.Initialise(_context);

        _accountService = new AccountService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void GetAccounts_CustomerExists_ReturnsAccounts()
    {
        int customerID = 2100;
        int accountNumFirstPos = 4100;
        int accountNumSecondPos = 4101;

        List<Account> accounts = _accountService.GetAccounts(customerID);

        Assert.NotNull(accounts);
        Assert.True(accounts.Count == 2);
        Assert.Equal(accounts[0].AccountNumber, accountNumFirstPos);
        Assert.Equal(accounts[1].AccountNumber, accountNumSecondPos);
    }

    [Fact]
    public void GetAccounts_CustomerDoesntExist_ReturnsNull()
    {
        int customerID = 5000;

        List<Account> accounts = _accountService.GetAccounts(customerID);

        Assert.Null(accounts);
    }

    [Fact]
    public void GetAccount_AccountNumExists_ReturnsAccount()
    {
        int accountNum = 4100;

        Account account = _accountService.GetAccount(accountNum);

        Assert.NotNull(account);
        Assert.Equal(accountNum, account.AccountNumber);
    }

    [Fact]
    public void GetAccount_AccountNumDoesntExists_ReturnsNull()
    {
        int accountNum = 4100600;

        Account account = _accountService.GetAccount(accountNum);

        Assert.Null(account);
    }

    [Fact]
    public void GetAccountWithValidationResult_AccountNumExists_ReturnsAccount()
    {
        int accountNum = 4100;

        (ValidationResult error, Account account) = _accountService.GetAccount(accountNum, "DestinationNumber");

        Assert.Null(error);
        Assert.NotNull(account);
        Assert.Equal(accountNum, account.AccountNumber);
    }

    [Theory]
    [InlineData(410660, "DestinationNumber")]
    [InlineData(4, "AccountNumber")]
    public void GetAccountWithValidationResult_AccountNumDoesntExists_ReturnsError(int accountNum, string propertyName)
    {
        (ValidationResult error, Account account) = _accountService.GetAccount(accountNum, propertyName);

        Assert.Null(account);
        Assert.NotNull(error);
        Assert.Equal(propertyName, error.MemberNames.First());
    }
}