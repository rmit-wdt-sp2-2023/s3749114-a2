using Microsoft.AspNetCore.Mvc;
using WebApi.Models.DataManager;
using BankLibrary.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillPayController : ControllerBase
{
    private readonly BillPayManager _repo;

    public BillPayController(BillPayManager repo) => _repo = repo;

    // GET /billpay
    // Return value: List<BillPay>
    // Description: Retrieves all BillPays.

    [HttpGet]
    public IEnumerable<BillPay> Get()
    {
        return _repo.GetAll();
    }

    // GET /billpay/{id}
    // Return value: BillPay
    // Description: Retrieves a BillPay with the given BillPayID.

    [HttpGet("{id}")]
    public BillPay Get(int id)
    {
        return _repo.Get(id);
    }

    // PUT /billpay/{billPay}
    // Return value: None
    // Description: Update the details of the given BillPay.

    [HttpPut]
    public void Put([FromBody] BillPay billPay)
    {
        _repo.Update(billPay.BillPayID, billPay);
    }
}

