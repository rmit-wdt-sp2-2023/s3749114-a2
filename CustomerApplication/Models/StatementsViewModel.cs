﻿using System.ComponentModel.DataAnnotations;
using BankLibrary.Models;

namespace CustomerApplication.Models;

public class StatementsViewModel
{
    [Required]
    [Display(Name = "Account Number")]
    [Range(1000, 9999, ErrorMessage = "You must select an Account.")]
    public int? AccountNumber { get; set; }

    public List<AccountViewModel> AccountsViewModel { get; set; }

    public List<Transaction> Transactions { get; set; }

    public int PageNumber { get; set; }

    public int? TotalPages { get; set; }

    public int? TransactionPages { get; set; }
}

