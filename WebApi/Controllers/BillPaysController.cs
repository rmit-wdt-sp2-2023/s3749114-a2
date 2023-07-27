using Microsoft.AspNetCore.Mvc;
using WebApi.Models.DataManager;
using BankLibrary.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillPaysController : ControllerBase
{
    private readonly BillPayManager _repo;

    public BillPaysController(BillPayManager repo) => _repo = repo;

    [HttpGet]
    public IEnumerable<BillPay> Get()
    {
        return _repo.GetAll();
    }

    [HttpGet("{id}")]
    public BillPay Get(int id)
    {
        return _repo.Get(id);
    }

    [HttpPost]
    public void Post([FromBody] BillPay billPay)
    {
        _repo.Add(billPay);
    }

    [HttpPut]
    public void Put([FromBody] BillPay billPay)
    {
        _repo.Update(billPay.BillPayID, billPay);
    }

    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}

