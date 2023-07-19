using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApplication.Models;

public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Range(1000, 9999)]
    public required int CustomerID { get; init; }

    [Required]
    [StringLength(50)]
    public required string Name { get; set; }

    [StringLength(11)]
    [RegularExpression(@"^\d{3} \d{3} \d{3}$")]
    public string TFN { get; set; }

    [StringLength(50)]
    public string Address { get; set; }

    [StringLength(40)]
    public string City { get; set; }

    [StringLength(3)]
    [RegularExpression(@"^(?i)(NSW|ACT|QLD|VIC|TAS|SA|WA|NT)$")]
    public string State { get; set; }

    [StringLength(4)]
    [RegularExpression(@"^\d{4}$")]
    public string PostCode { get; set; }

    [StringLength(12)]
    [RegularExpression(@"^04\d{2} \d{3} \d{3}$")]
    public string Mobile { get; set; }

    public virtual List<Account> Accounts { get; init; } = new();
}