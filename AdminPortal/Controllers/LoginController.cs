using AdminPortal.Filters;
using Microsoft.AspNetCore.Mvc;
using AdminPortal.ViewModels;
using BankLibrary.Models;
using Newtonsoft.Json;
using AdminPortal.Mappers;
using System.Text;

namespace AdminPortal.Controllers;

// Controller for actions relating to customer logins, not the admin login. 

[AuthorizeAdmin]
public class LoginController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    private HttpClient Client => _clientFactory.CreateClient("api");

    public LoginController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public IActionResult Index() => View(new LoginSearchViewModel());

    [HttpPost]
    public IActionResult Search(LoginSearchViewModel loginSearchVM)
    {
        if (!ModelState.IsValid)
            return View("Index", loginSearchVM);

        HttpResponseMessage response = Client.GetAsync($"api/login/{loginSearchVM.LoginID}").Result;

        if (!response.IsSuccessStatusCode)
            return View("RequestError");

        string result = response.Content.ReadAsStringAsync().Result;

        Login login = JsonConvert.DeserializeObject<Login>(result);

        if (login is not null)
            return View("Profile", ViewModelMapper.LoginProfile(login));

        ModelState.AddModelError("LoginID", "No login found.");

        return View("Index", loginSearchVM);
    }

    [HttpPost]
    public IActionResult UpdateStatus(LoginProfileViewModel loginProfileVM)
    {
        if (!ModelState.IsValid)
            return View("Profile", loginProfileVM);
            
        HttpResponseMessage response = Client.GetAsync($"api/login/{loginProfileVM.LoginID}").Result;

        if (!response.IsSuccessStatusCode)
            return View("RequestError");

        string result = response.Content.ReadAsStringAsync().Result;

        Login login = JsonConvert.DeserializeObject<Login>(result);

        if (login is null)
            return View("RequestError");

        login.LoginStatus = loginProfileVM.LoginStatus;

        StringContent content = new(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

        response = Client.PutAsync("api/login", content).Result;

        if (!response.IsSuccessStatusCode)
            return View("RequestError");

        ViewBag.DisplaySuccess = true;

        return View("Profile", ViewModelMapper.LoginProfile(login));
    }
}