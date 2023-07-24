using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.Models;

public class Payee
{
    public int PayeeID { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string Address { get; set; }

    [Required]
    [StringLength(40)]
    public string City { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 2)]
    public string State { get; set; }

    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string PostCode { get; set; }

    [Required]
    [StringLength(14, MinimumLength = 14)]
    [RegularExpression(@"^\(0\d\) \d{4} \d{4}$")]
    public string Phone { get; set; }
}