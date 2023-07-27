using CustomerApplication.Filters;
using BankLibrary.Models;
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
    public IActionResult Confirm(CreateTransactionViewModel transVM)
    {
        List<ValidationResult> errors = null;

        if (transVM.TransactionType == TransactionType.Deposit)
            errors = _transactionService.ConfirmDeposit(transVM.AccountNumber.GetValueOrDefault(),
                transVM.Amount.GetValueOrDefault(), transVM.Comment);

        else if (transVM.TransactionType == TransactionType.Withdraw)
            errors = _transactionService.ConfirmWithdraw(transVM.AccountNumber.GetValueOrDefault(),
                transVM.Amount.GetValueOrDefault(), transVM.Comment);

        else if(transVM.TransactionType == TransactionType.Transfer)
            errors = _transactionService.ConfirmTransfer(transVM.AccountNumber.GetValueOrDefault(),
                transVM.DestinationNumber, transVM.Amount.GetValueOrDefault(), transVM.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Index), ViewModelMapper.CreateTransaction(
                transVM.TransactionType, _accountService.GetAccounts(CustomerID), transVM));

        return View("Confirm", transVM);
    }

    // Submitting transactions

    [HttpPost]
    public IActionResult Submit(CreateTransactionViewModel transVM)
    {
        List<ValidationResult> errors = null;
        List<Transaction> transactions = null;

        if (transVM.TransactionType == TransactionType.Deposit)
        {
            (errors, Transaction transaction) = _transactionService.SubmitDeposit(
                transVM.AccountNumber.GetValueOrDefault(), transVM.Amount.GetValueOrDefault(), transVM.Comment);

            if (transaction is not null)
                transactions = new() { transaction };
        }
        else if (transVM.TransactionType == TransactionType.Withdraw)
            (errors, transactions) = _transactionService.SubmitWithdraw(
                transVM.AccountNumber.GetValueOrDefault(), transVM.Amount.GetValueOrDefault(), transVM.Comment);
            
        else if (transVM.TransactionType == TransactionType.Transfer)
            (errors, transactions) = _transactionService.SubmitTransfer(transVM.AccountNumber.GetValueOrDefault(),
                transVM.DestinationNumber, transVM.Amount.GetValueOrDefault(), transVM.Comment);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(Index), ViewModelMapper.CreateTransaction(
                transVM.TransactionType, _accountService.GetAccounts(CustomerID), transVM));

        return View("Success", ViewModelMapper.Receipt(transactions));
    }
}