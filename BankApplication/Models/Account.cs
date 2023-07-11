using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApplication.Models;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account number")]
    [Range(1000, 9999)]
    public int AccountNumber { get; set; }

    [Required]
    [Display(Name = "Type")]
    public AccountType AccountType { get; set; }

    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [NotMapped]
    public virtual List<Transaction> Transactions { get; set; }
}