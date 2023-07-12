using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLibrary.Models;

public class Transaction
{
    public int TransactionID { get; set; }

    [Required]
    public TransactionType TransactionType { get; set; }

    [ForeignKey("Account")]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("DestinationAccount")]
    public int? DestinationNumber { get; set; }
    public virtual Account DestinationAccount { get; set; }

    [Required]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [StringLength(30)]
    public string Comment { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime TransactionTimeUtc { get; set; } 
}