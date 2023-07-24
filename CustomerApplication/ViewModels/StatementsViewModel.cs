using System.ComponentModel.DataAnnotations;
using CustomerApplication.Models;
using X.PagedList;

namespace CustomerApplication.ViewModels;

public class StatementsViewModel
{
    [Required]
    [Display(Name = "Account Number")]
    [Range(1000, 9999, ErrorMessage = "You must select an Account.")]
    public int? AccountNumber { get; set; }

    public List<AccountViewModel> AccountsViewModel { get; set; }

    public IPagedList<Transaction> Transactions { get; set; }
}