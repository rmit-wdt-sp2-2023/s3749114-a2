using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLibrary.Models;

public class Login
{
    [StringLength(8)]
    [RegularExpression(@"^\d{8}$")]
    [Column(TypeName = "char")]
    public required string LoginID { get; init; }

    [ForeignKey("Customer")]
    [Range(1000, 9999)]
    public required int CustomerID { get; init; }
    public virtual Customer Customer { get; init; }

    [Required]
    [StringLength(94)]
    [Column(TypeName = "char")]
    public required string PasswordHash { get; set; }

    [Required]
    [Display(Name = "Login status")]
    public LoginStatus LoginStatus { get; set; } = LoginStatus.Unlocked;
}