using System.ComponentModel.DataAnnotations;
using CustomerApplication.Models;
using X.PagedList;

namespace CustomerApplication.ViewModels;

public class StatementsViewModel
{
    [Required(ErrorMessage = "You must select an account.")]
    [Display(Name = "Account Number")]
    [Range(1000, 9999, ErrorMessage = "Invalid account number.")]
    public int? AccountNumber { get; set; }

    public List<AccountViewModel> AccountViewModels { get; set; } = new();

    public IPagedList<Transaction> Transactions { get; set; } 
}