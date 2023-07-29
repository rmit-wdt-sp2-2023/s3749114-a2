using BankLibrary.Data;
using BankLibrary.Models;
using WebApi.Models.Repository;

namespace WebApi.Models.DataManager;

public class CustomerManager : IDataRepository<Customer, int>
{
    private readonly BankContext _context;

    public CustomerManager(BankContext context)
    {
        _context = context;
    }

    public Customer Get(int id)
    {
        return _context.Customers.Find(id);
    }

    public IEnumerable<Customer> GetAll()
    {
        return _context.Customers.ToList();
    }

    public int Update(int id, Customer customer)
    {
        _context.Update(customer);
        _context.SaveChanges();
            
        return id;
    }
}
