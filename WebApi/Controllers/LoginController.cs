using Microsoft.AspNetCore.Mvc;
using WebApi.Models.DataManager;
using BankLibrary.Models;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginManager _repo;

    public LoginController(LoginManager repo) => _repo = repo;

    [HttpGet]
    public IEnumerable<Login> Get()
    {
        return _repo.GetAll();
    }

    [HttpGet("{id}")]
    public Login Get(string id)
    {
        return _repo.Get(id);
    }

    [HttpPut]
    public void Put([FromBody] Login login)
    {
        _repo.Update(login.LoginID, login);
    }
}