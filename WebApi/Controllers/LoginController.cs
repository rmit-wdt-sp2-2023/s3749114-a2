using Microsoft.AspNetCore.Mvc;
using WebApi.Models.DataManager;
using BankLibrary.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly LoginManager _repo;

    public LoginController(LoginManager repo) => _repo = repo;

    //GET /login
    //Return value: List<Login>
    //Description: Retrieves all Logins.

    [HttpGet]
    public IEnumerable<Login> Get()
    {
        return _repo.GetAll();
    }

    // GET /login/{id}
    // Return value: Login
    // Description: Retrieves a Logins with the given LoginID.

    [HttpGet("{id}")]
    public Login Get(string id)
    {
        return _repo.Get(id);
    }

    // PUT /login/{login}
    // Return value: None
    // Description: Update the details of the given Login.

    [HttpPut]
    public void Put([FromBody] Login login)
    {
        _repo.Update(login.LoginID, login);
    }
}