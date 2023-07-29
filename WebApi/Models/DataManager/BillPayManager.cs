using BankLibrary.Data;
using BankLibrary.Models;
using WebApi.Models.Repository;

namespace WebApi.Models.DataManager;

public class BillPayManager : IDataRepository<BillPay, int>
{
    private readonly BankContext _context;

    public BillPayManager(BankContext context) => _context = context;

    public BillPay Get(int id)
    {
        return _context.BillPays.Find(id);
    }

    public IEnumerable<BillPay> GetAll()
    {
        return _context.BillPays.ToList();
    }

    public int Update(int id, BillPay billPay)
    {
        _context.Update(billPay);
        _context.SaveChanges();

        return id;
    }
}
