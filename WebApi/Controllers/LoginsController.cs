using Microsoft.AspNetCore.Mvc;
using WebApi.Models.DataManager;
using BankLibrary.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginsController : ControllerBase
{
    private readonly LoginManager _repo;

    public LoginsController(LoginManager repo) => _repo = repo;

    [HttpGet]
    public IEnumerable<Login> Get()
    {
        return _repo.GetAll();
    }

    [HttpGet("{id}")]
    public Login Get(int id)
    {
        return _repo.Get(id);
    }

    [HttpPost]
    public void Post([FromBody] Login login)
    {
        _repo.Add(login);
    }

    [HttpPut]
    public void Put([FromBody] Login login)
    {
        _repo.Update(login.CustomerID, login);
    }

    [HttpDelete("{id}")]
    public long Delete(int id)
    {
        return _repo.Delete(id);
    }
}