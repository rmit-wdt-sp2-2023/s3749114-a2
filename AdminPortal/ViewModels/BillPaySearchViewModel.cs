using System.ComponentModel.DataAnnotations;

namespace AdminPortal.ViewModels;

public class BillPaySearchViewModel
{
    [Required]
    public int? BillPayID { get; set; }
}

