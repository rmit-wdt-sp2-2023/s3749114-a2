using System.ComponentModel.DataAnnotations;
using CustomerApplication.Data;
using CustomerApplication.Models;
using SimpleHashing.Net;

namespace CustomerApplication.Services;

public class LoginService
{
    private readonly BankContext _context;

    private static readonly ISimpleHash SimpleHash = new SimpleHash();

    public LoginService(BankContext context) => _context = context;

    public Login Login(string loginID, string password)
    {
        Login login = _context.Logins.Find(loginID);

        if (login is not null)
            if (SimpleHash.Verify(password, login.PasswordHash))
                return login;

        return null;
    }

    public List<ValidationResult> ChangePassword(int customerID, string oldPass, string newPass, string confirmPass)
    {
        List<ValidationResult> errors = new();

        if (oldPass is null)
            errors.Add(new ValidationResult("Enter old password.", new List<string>() { "OldPassword" }));

        if (newPass is null)
            errors.Add(new ValidationResult("Enter new password.", new List<string>() { "NewPassword" }));

        if (newPass != confirmPass)
            errors.Add(new ValidationResult("Passwords don't match.", new List<string>() { "ConfirmPassword" }));

        if (errors.Count > 0)
            return errors;

        Login login = _context.Logins.FirstOrDefault(c => c.CustomerID == customerID);

        if (login is null)
            errors.Add(new ValidationResult("Error, couldn't find customer.", new List<string>() { "PasswordFailed" }));
        else
            if (!SimpleHash.Verify(oldPass, login.PasswordHash))
            errors.Add(new ValidationResult("Incorrect password.", new List<string>() { "OldPassword" }));

        if (errors.Count > 0)
            return errors;

        login.PasswordHash = SimpleHash.Compute(newPass);

        _context.Logins.Update(login);
        _context.SaveChanges();

        return null;
    }
}