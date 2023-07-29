using Microsoft.AspNetCore.Mvc;
using AdminPortal.Filters;
using AdminPortal.ViewModels;
using BankLibrary.Models;
using Newtonsoft.Json;
using System.Text;

namespace AdminPortal.Controllers;

[AuthorizeAdmin]
public class BillPayController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    private HttpClient Client => _clientFactory.CreateClient("api");

    public BillPayController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    public IActionResult Index() => View(new BillPaySearchViewModel());

    [HttpPost]
    public IActionResult Search(BillPaySearchViewModel searchVM)
    {
        if (!ModelState.IsValid)
            return View("Index", searchVM);

        HttpResponseMessage response = Client.GetAsync($"api/billpay/{searchVM.BillPayID}").Result;

        if (!response.IsSuccessStatusCode)
            return View("RequestError");

        string result = response.Content.ReadAsStringAsync().Result;

        BillPay billPay = JsonConvert.DeserializeObject<BillPay>(result);

        if (billPay is not null)
            return View("Details", billPay);

        ModelState.AddModelError("BillPayID", "No BillPay found.");

        return View("Index", searchVM);
    }

    [HttpPost]
    public IActionResult UpdateStatus(BillPay billPay)
    {
        if (!ModelState.IsValid)
            return View("Details", billPay);

        StringContent content = new(JsonConvert.SerializeObject(billPay), Encoding.UTF8, "application/json");

        HttpResponseMessage response = Client.PutAsync("api/billpay", content).Result;

        if (!response.IsSuccessStatusCode)
            return View("RequestError");

        ViewBag.DisplaySuccess = true;

        return View("Details", billPay);
    }
}

