using Microsoft.AspNetCore.Mvc;
using AdminPortal.Filters;
using AdminPortal.ViewModels;
using Newtonsoft.Json;
using BankLibrary.Models;
using System.Text;

namespace AdminPortal.Controllers;

[AuthorizeAdmin]
public class CustomerController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    private HttpClient Client => _clientFactory.CreateClient("api");

    public CustomerController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public IActionResult Index() => View(new CustomerSearchViewModel());

    [HttpPost]
    public IActionResult Search(CustomerSearchViewModel searchVM)
    {
        if (!ModelState.IsValid)
            return View("Index", searchVM);

        HttpResponseMessage response = Client.GetAsync($"api/customer/{searchVM.CustomerID}").Result;

        if (!response.IsSuccessStatusCode)
            return View("RequestError");

        string result = response.Content.ReadAsStringAsync().Result;

        Customer customer = JsonConvert.DeserializeObject<Customer>(result);

        if (customer is not null)
            return View("Profile", customer);

        ModelState.AddModelError("CustomerID", "No customer found.");

        return View("Index", searchVM);
    }

    public IActionResult Profile(Customer customer) => View(customer);

    public IActionResult Edit(int id)
    {
        HttpResponseMessage response = Client.GetAsync($"api/customer/{id}").Result;

        if (!response.IsSuccessStatusCode)
            return View("RequestError");

        string result = response.Content.ReadAsStringAsync().Result;

        Customer customer = JsonConvert.DeserializeObject<Customer>(result);

        if (customer is null)
            return RedirectToAction(nameof(Index));

        return View(customer);    
    }

    [HttpPost]
    public IActionResult Submit(Customer customer)
    {
        if (!ModelState.IsValid)
            return View("Edit", customer);

        StringContent content = new(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");

        HttpResponseMessage response = Client.PutAsync("api/customer", content).Result;

        if (!response.IsSuccessStatusCode)
            return View("RequestError");

        ViewBag.DisplaySuccess = true;

        return View("Profile", customer);
    }
}