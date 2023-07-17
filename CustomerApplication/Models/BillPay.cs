using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApplication.Models;

public class BillPay
{
    public int BillPayID { get; set; }

    [ForeignKey("Account")]
    public int AccountNumber { get; set; }
    public virtual Account Account { get; set; }

    [ForeignKey("Payee")]
    public int PayeeID { get; set; }
    public virtual Payee Payee { get; set; }

    [Required]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ScheduledTimeUtc { get; set; }

    [Required]
    [Column(TypeName = "char")]
    public Period Period { get; set; }
}