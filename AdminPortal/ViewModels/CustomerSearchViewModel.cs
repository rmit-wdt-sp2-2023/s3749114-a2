using System.ComponentModel.DataAnnotations;

namespace AdminPortal.ViewModels;

public class CustomerSearchViewModel
{
    [Required(ErrorMessage ="You must enter a customer ID.")]
    [Range(1000, 9999, ErrorMessage = "Invalid customer ID.")]
    public int? CustomerID { get; set; }
}