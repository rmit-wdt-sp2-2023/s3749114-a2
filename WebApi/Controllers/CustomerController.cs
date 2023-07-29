using Microsoft.AspNetCore.Mvc;
using WebApi.Models.DataManager;
using BankLibrary.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Castle.Core.Resource;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerManager _repo;

    public CustomerController(CustomerManager repo) => _repo = repo;

    // GET /customer
    // Return value: List<Customer>
    // Description: Retrieves all Customers.

    [HttpGet("all")]
    public IEnumerable<Customer> Get()
    {
        return _repo.GetAll();
    }

    //GET /customer{id}
    //Return value: Customer
    //Description: Retrieves a Customer with the given CustomerID.

    [HttpGet("{id}")]
    public Customer Get(int id)
    {
        return _repo.Get(id);
    }

    //PUT /customer{customer}
    //Return value: None
    //Description: Update the details of the given Customer.

    [HttpPut]
    public void Put([FromBody] Customer customer)
    {
        _repo.Update(customer.CustomerID, customer);
    }
}