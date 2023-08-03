
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLibrary.Models;

public class ProfilePicture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [ForeignKey("Customer")]
    [Range(1000, 9999)]
    public required int CustomerID { get; init; }
    public virtual Customer Customer { get; init; }

    public byte[] Image { get; set; }

    public string ContentType { get; set; } = "image/jpg";

    public string FileName { get; set; }
}