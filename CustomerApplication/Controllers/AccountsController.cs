using CustomerApplication.Filters;
using CustomerApplication.Models;
using CustomerApplication.Services;
using CustomerApplication.ViewModels;
using CustomerApplication.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class AccountsController : Controller
{
    private readonly AccountService _accountService;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public AccountsController(AccountService accountService) => _accountService = accountService;

    public IActionResult Index() => View(ViewModelMapper.Accounts(_accountService.GetAccounts(CustomerID)));
}