using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankLibrary.Validation;

namespace BankLibrary.Models;

public class Transaction
{
    public int TransactionID { get; init; }

    [Required]
    public required TransactionType TransactionType { get; init; }

    [ForeignKey("Account")]
    public required int AccountNumber { get; init; }
    public virtual Account Account { get; init; }

    [ForeignKey("DestinationAccount")]
    public int? DestinationNumber { get; init; } = null;
    public virtual Account DestinationAccount { get; set; }

    [Required]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public required decimal Amount { get; init; }

    [StringLength(30)]
    public string Comment { get; init; } = null;

    [Required]
    [DataType(DataType.Date)]
    public DateTime TransactionTimeUtc { get; init; } = DateTime.UtcNow;
}