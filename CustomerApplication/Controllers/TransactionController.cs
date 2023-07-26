using CustomerApplication.Filters;
using CustomerApplication.Models;
using CustomerApplication.Services;
using CustomerApplication.ViewModels;
using CustomerApplication.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class TransactionController : Controller
{
    private readonly TransactionService _transactionService;

    private readonly AccountService _accountService;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public TransactionController(TransactionService transactionService, AccountService accountService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
    }

    // Creating transactions

    public IActionResult Deposit() => View(nameof(Index),
        ViewModelMapper.CreateTransaction(TransactionType.Deposit, _accountService.GetAccounts(CustomerID)));

    public IActionResult Withdraw() => View(nameof(Index),
        ViewModelMapper.CreateTransaction(TransactionType.Withdraw, _accountService.GetAccounts(CustomerID)));

    public IActionResult Transfer() => View(nameof(Index),
        ViewModelMapper.CreateTransaction(TransactionType.Transfer, _accountService.GetAccounts(CustomerID)));

    // Confirming transactions 

    [HttpPost]
    public IActionResult ConfirmDeposit(CreateTransactionViewModel createTransactionVM)
    {
        List<ValidationResult> errors = _transactionService.ConfirmDeposit(
            createTransactionVM.AccountNumber.GetValueOrDefault(),
            createTransactionVM.Amount.GetValueOrDefault(), createTransactionVM.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);
            
        if (!ModelState.IsValid)
            return View(nameof(Index), ViewModelMapper.CreateTransaction(
                    TransactionType.Deposit, _accountService.GetAccounts(CustomerID), createTransactionVM));

        return View("Confirm", createTransactionVM);
    }

    [HttpPost]
    public IActionResult ConfirmWithdraw(CreateTransactionViewModel createTransactionVM)
    {
        List<ValidationResult> errors = _transactionService.ConfirmWithdraw(
            createTransactionVM.AccountNumber.GetValueOrDefault(),
            createTransactionVM.Amount.GetValueOrDefault(), createTransactionVM.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Index), ViewModelMapper.CreateTransaction(
                TransactionType.Withdraw, _accountService.GetAccounts(CustomerID), createTransactionVM));

        return View("Confirm", createTransactionVM);
    }

    [HttpPost]
    public IActionResult ConfirmTransfer(CreateTransactionViewModel createTransactionVM)
    {
        List<ValidationResult> errors = _transactionService.ConfirmTransfer(
            createTransactionVM.AccountNumber.GetValueOrDefault(), createTransactionVM.DestinationNumber,
            createTransactionVM.Amount.GetValueOrDefault(), createTransactionVM.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Index), ViewModelMapper.CreateTransaction(
                TransactionType.Transfer, _accountService.GetAccounts(CustomerID), createTransactionVM));

        return View("Confirm", createTransactionVM);
    }

    // Submitting transactions

    [HttpPost]
    public IActionResult SubmitDeposit(CreateTransactionViewModel createTransactionVM)
    {
        (List<ValidationResult> errors, Transaction transaction) = _transactionService.SubmitDeposit(
            createTransactionVM.AccountNumber.GetValueOrDefault(),
            createTransactionVM.Amount.GetValueOrDefault(), createTransactionVM.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Index), ViewModelMapper.CreateTransaction(
                TransactionType.Deposit, _accountService.GetAccounts(CustomerID), createTransactionVM));

        return View("Success", ViewModelMapper.Receipt(new List<Transaction>() { transaction }));
    }

    [HttpPost]
    public IActionResult SubmitWithdraw(CreateTransactionViewModel createTransactionVM)
    {
        (List<ValidationResult> errors, List <Transaction> transactions) =
            _transactionService.SubmitWithdraw(createTransactionVM.AccountNumber.GetValueOrDefault(),
            createTransactionVM.Amount.GetValueOrDefault(), createTransactionVM.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Index), ViewModelMapper.CreateTransaction(
                TransactionType.Withdraw, _accountService.GetAccounts(CustomerID), createTransactionVM));

        return View("Success", ViewModelMapper.Receipt(transactions));
    }

    [HttpPost]
    public IActionResult SubmitTransfer(CreateTransactionViewModel createTransactionVM)
    {
        (List<ValidationResult> errors, List<Transaction> transactions) = _transactionService.SubmitTransfer(
            createTransactionVM.AccountNumber.GetValueOrDefault(), createTransactionVM.DestinationNumber,
            createTransactionVM.Amount.GetValueOrDefault(), createTransactionVM.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Index), ViewModelMapper.CreateTransaction(
                TransactionType.Transfer, _accountService.GetAccounts(CustomerID), createTransactionVM));

        return View("Success", ViewModelMapper.Receipt(transactions));
    }
}