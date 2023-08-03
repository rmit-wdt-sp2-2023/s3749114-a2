using System.ComponentModel.DataAnnotations;
using BankLibrary.Data;
using BankLibrary.Models;
using CustomerApplication.Data;
using CustomerApplication.Services;
using Microsoft.EntityFrameworkCore;
using SimpleHashing.Net;
using Xunit;

namespace CustomerApplication.Tests.Services;

public class LoginServiceTests
{
    private readonly BankContext _context;

    private readonly LoginService _loginService;

    public LoginServiceTests()
    {
        _context = new BankContext(new DbContextOptionsBuilder<BankContext>()
            .UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);

        _context.Database.EnsureCreated();

        SeedData.Initialise(_context);

        _loginService = new LoginService(_context);
    }

    [Fact]
    public void Login_ValidLogin_ReturnLogin()
    {
        string loginID = "12345678";
        string password = "abc123";

        (ValidationResult error, Login login) = _loginService.Login(loginID, password);

        Assert.Null(error);
        Assert.NotNull(login);
        Assert.Equal(loginID, login.LoginID);
    }

    [Fact]
    public void Login_ValidLoginLocked_ReturnError()
    {
        string loginID = "38074569";
        string password = "ilovermit2020";

        Login loginToLock = _context.Logins.Find(loginID);
        loginToLock.LoginStatus = LoginStatus.Locked;
        _context.Logins.Update(loginToLock);
        _context.SaveChanges();

        (ValidationResult error, Login login) = _loginService.Login(loginID, password);

        Assert.Null(login);
        Assert.NotNull(error);
    }

    [Theory]
    [InlineData("12345678", "password")]
    [InlineData("12345670", "abc123")]
    [InlineData("456", "hello")]
    [InlineData("dfsdfsd", "hello")]
    public void Login_InvalidParameters_ReturnError(string loginID, string password)
    {
        (ValidationResult error, Login login) = _loginService.Login(loginID, password);

        Assert.Null(login);
        Assert.NotNull(error);
    }

    [Fact]
    public void ChangePassword_ValidParameters_DatabaseUpdated()
    {
        int customerID = 2300;
        string oldPass = "youWill_n0tGuess-This!";
        string newPass = "newPassword";
        string confirmPass = "newPassword";

        List<ValidationResult> errors = _loginService.ChangePassword(customerID, oldPass, newPass, confirmPass);

        Assert.True(new SimpleHash().Verify(newPass, _context.Logins.Find("17963428").PasswordHash));
        Assert.Null(errors);
    }

    [Theory]
    [InlineData(2300, "youWill_n0tGuess-This!", "newPassword", "PassDoesntMatch")]
    [InlineData(2300, "youWill_n0tGuess-This!", "PassDoesntMatch", "newPassword")]
    [InlineData(2300, "IncorrectPass", "newPassword", "newPassword")]
    [InlineData(23023450, "youWill_n0tGuess-This!", "newPassword", "newPassword")]
    public void ChangePassword_InvalidParameters_ReturnsErrors(
        int customerID, string oldPass, string newPass, string confirmPass)
    {
        List<ValidationResult> errors = _loginService.ChangePassword(customerID, oldPass, newPass, confirmPass);

        Assert.NotNull(errors);
    }
}

