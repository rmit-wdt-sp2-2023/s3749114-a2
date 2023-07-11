using BankApplication.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApplication.Dtos;

// DTO class to represent customer data in the JSON.

public class CustomerDto
{
    public int CustomerID { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PostCode { get; set; }
    public List<AccountDto> Accounts { get; set; }
    public LoginDto Login { get; set; }
}