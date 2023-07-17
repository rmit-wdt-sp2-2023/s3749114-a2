using System.ComponentModel.DataAnnotations;

namespace CustomerApplication.ViewModels;

public class CustomerViewModel
{
    [Required]
    [StringLength(50, ErrorMessage = "Cannot be more than 50 characters.")]
    public string Name { get; set; }

    [StringLength(11, MinimumLength = 11, ErrorMessage = "Must be exactly 11 characters.")]
    [RegularExpression(@"^\d{3} \d{3} \d{3}$", ErrorMessage = "Must contain digits in the specified format.")]
    public string TFN { get; set; }

    [StringLength(50, ErrorMessage = "Cannot be more than 50 characters.")]
    public string Address { get; set; }

    [StringLength(40, ErrorMessage = "Cannot be more than 40 characters.")]
    public string City { get; set; }

    [StringLength(3, MinimumLength = 2, ErrorMessage = "Must be a 2 or 3 lettered Australian state.")]
    [RegularExpression(@"^(NT|QLD|NSW|ACT|VIC|TAS|SA|WA)$",
        ErrorMessage = "Must be a 2 or 3 lettered Australian state.")]
    public string State { get; set; }

    [StringLength(4, MinimumLength = 4, ErrorMessage = "Must be exactly 4 digits.")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Must contain digits only.")]
    public string PostCode { get; set; }

    [StringLength(12, MinimumLength = 12, ErrorMessage = "Must be exactly 12 characters.")]
    [RegularExpression(@"^04\d{2} \d{3} \d{3}$", ErrorMessage = "Must contain digits in the specified format.")]
    public string Mobile { get; set; }

    public bool DetailsUpdated { get; set; } = false;
}

