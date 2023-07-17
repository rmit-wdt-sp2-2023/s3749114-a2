using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApplication.Models;

public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Range(1000, 9999)]
    public int CustomerID { get; init; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [StringLength(11, MinimumLength = 11)]
    [RegularExpression(@"^\d{3} \d{3} \d{3}$")]
    public string TFN { get; set; }

    [StringLength(50)]
    public string Address { get; set; }

    [StringLength(40)]
    public string City { get; set; }

    [StringLength(3, MinimumLength = 2)]
    [RegularExpression(@"^(NT|QLD|NSW|ACT|VIC|TAS|SA|WA)$")]
    public string State { get; set; }

    [StringLength(4, MinimumLength = 4)]
    [RegularExpression(@"^\d{4}$")]
    public string PostCode { get; set; }

    [StringLength(12, MinimumLength = 12)]
    [RegularExpression(@"^04\d{2} \d{3} \d{3}$")]
    public string Mobile { get; set; }
    public virtual List<Account> Accounts { get; set; }
}