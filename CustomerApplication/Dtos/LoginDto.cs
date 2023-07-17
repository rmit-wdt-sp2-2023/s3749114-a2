namespace CustomerApplication.Dtos;

// DTO class to represent login data in the JSON.

public class LoginDto
{
    public string LoginID { get; set; }
    public string PasswordHash { get; set; }
}