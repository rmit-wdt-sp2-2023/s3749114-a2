using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLibrary.Models;

public class Login
{
    [StringLength(8)]
    [Column(TypeName = "char")]
    public string LoginID { get; set; }

    [ForeignKey("Customer")]
    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [Required]
    [StringLength(94)]
    [Column(TypeName = "char")]
    public string PasswordHash { get; set; }
}