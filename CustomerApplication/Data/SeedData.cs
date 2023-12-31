﻿using Newtonsoft.Json;
using BankLibrary.Models;
using CustomerApplication.Dtos;
using CustomerApplication.Mappers;
using CustomerApplication.Data;
using BankLibrary.Data;

namespace CustomerApplication.Data;

public static class SeedData
{
    public static void Initialise(IServiceProvider serviceProvider)
    {
        BankContext context = serviceProvider.GetRequiredService<BankContext>();

        Initialise(context);
    }

    public static void Initialise(BankContext context)
    {
        // Check if any customers already exist and if they do stop.

        if (context.Customers.Any())
            return;

        // Get JSON from web service.

        const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";
        using HttpClient client = new();
        string json = client.GetStringAsync(Url).Result;

        // Convert JSON into DTO.

        var customersDto = JsonConvert.DeserializeObject<List<CustomerDto>>(json, new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
        });

        // Convert DTO into business objects and insert into database.

        List<Customer> customers = DtoMapper.ConvertCustomersFromDto(customersDto);

        foreach (Customer customer in customers)
            context.Customers.Add(customer);

        List<Login> logins = DtoMapper.ConvertLoginsFromDto(customersDto);

        foreach (Login login in logins)
            context.Logins.Add(login);

        // Create and insert payees into database.

        context.Payees.Add(new Payee()
        {
            Name = "Optus",
            Address = "367 Collins Street",
            City = "Melbourne",
            State = "VIC",
            PostCode = "3000",
            Phone = "(03) 7022 8530"
        });
        context.Payees.Add(new Payee()
        {
            Name = "Telstra",
            Address = "246-260 Bourke Street",
            City = "Melbourne",
            State = "VIC",
            PostCode = "3000",
            Phone = "(03) 8647 4954"
        });
        context.SaveChanges();
    }
}