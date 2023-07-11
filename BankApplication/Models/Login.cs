namespace BankApplication.Models;

public class Login
{
    public string LoginID { get; set; }

    public int CustomerID { get; set; }

    public string PasswordHash { get; set; }
}