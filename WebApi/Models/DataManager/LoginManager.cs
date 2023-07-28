using BankLibrary.Data;
using BankLibrary.Models;
using WebApi.Models.Repository;

namespace WebApi.Models.DataManager;

public class LoginManager : IDataRepository<Login, string>
{
    private readonly BankContext _context;

    public LoginManager(BankContext context)
    {
        _context = context;
    }

    public Login Get(string id)
    {
        return _context.Logins.Find(id);
    }

    public IEnumerable<Login> GetAll()
    {
        return _context.Logins.ToList();
    }

    public string Update(string id, Login login)
    {
        _context.Update(login);
        _context.SaveChanges();

        return id;
    }
}
