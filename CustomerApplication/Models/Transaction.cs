﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CustomerApplication.Validation;

namespace CustomerApplication.Models;

public class Transaction
{
    [Display(Name = "ID")]
    public int TransactionID { get; init; }

    [Required]
    [Display(Name = "Type")]
    public required TransactionType TransactionType { get; init; }

    [ForeignKey("Account")]
    [Range(1000, 9999)]
    [Display(Name = "Account number")]
    public required int AccountNumber { get; init; }
    public virtual Account Account { get; init; }

    [ForeignKey("DestinationAccount")]
    [Range(1000, 9999)]
    [Display(Name = "Destination number")]
    public int? DestinationNumber { get; init; } = null;
    public virtual Account DestinationAccount { get; init; }

    [Required]
    [Column(TypeName = "money")]
    [CustomValidation(typeof(ValidationMethods), "MoreThanTwoDecimalPlaces")]
    [CustomValidation(typeof(ValidationMethods), "GreaterThanZero")]
    [DataType(DataType.Currency)]
    public required decimal Amount { get; init; }

    [StringLength(30)]
    public string Comment { get; init; }

    [Required]
    [Display(Name = "Time")]
    public DateTime TransactionTimeUtc { get; init; } = DateTime.UtcNow;

    public string LocalTimeString() =>
        TimeZoneInfo.ConvertTimeFromUtc(TransactionTimeUtc, TimeZoneInfo.Local).ToString("dd/MM/yyyy hh:mm tt").ToUpper();
}