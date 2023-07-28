using Microsoft.AspNetCore.Mvc;
using WebApi.Models.DataManager;
using BankLibrary.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerManager _repo;

    public CustomerController(CustomerManager repo) => _repo = repo;

    [HttpGet("all")]
    public IEnumerable<Customer> Get()
    {
        return _repo.GetAll();
    }

    [HttpGet("{id}")]
    public Customer Get(int id)
    {
        return _repo.Get(id);
    }

    [HttpPut]
    public void Put([FromBody] Customer customer)
    {
        _repo.Update(customer.CustomerID, customer);
    }
}