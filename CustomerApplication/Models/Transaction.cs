using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApplication.Models;

public class Transaction
{
    [Display(Name = "ID")]
    public int TransactionID { get; init; }

    [Required]
    [Display(Name = "Transaction Type")]
    public required TransactionType TransactionType { get; init; }

    [ForeignKey("Account")]
    [Display(Name = "Account Number")]
    public required int AccountNumber { get; init; }
    public virtual Account Account { get; init; }

    [ForeignKey("DestinationAccount")]
    [Display(Name = "Destination Number")]
    public int? DestinationNumber { get; init; } = null;
    public virtual Account DestinationAccount { get; set; }

    [Required]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public required decimal Amount { get; init; }

    [StringLength(30)]
    public string Comment { get; init; } = null;

    [Required]
    [Display(Name = "Time")]
    public DateTime TransactionTimeUtc { get; init; } = DateTime.UtcNow;

    public string LocalTimeString() =>
        TimeZoneInfo.ConvertTimeFromUtc(TransactionTimeUtc, TimeZoneInfo.Local).ToString("dd/MM/yyyy hh:mm tt").ToUpper();
}