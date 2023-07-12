using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLibrary.Models;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account number")]
    [Range(1000, 9999)]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Account type")]
    public AccountType AccountType { get; set; }

    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }
}