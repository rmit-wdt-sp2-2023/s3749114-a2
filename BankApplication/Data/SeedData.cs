using BankApplication.Models;
using BankApplication.Dtos;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BankApplication.Mappers;
//using System.Transactions;

namespace BankApplication.Data;

public static class SeedData
{
    public static void Initialise(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<BankApplicationContext>();

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
        List<Login> logins = DtoMapper.ConvertLoginsFromDto(customersDto);

        // Insert customers and logins into database.

        foreach (Customer customer in customers)
            context.Customers.Add(customer);
        foreach (Login login in logins)
            context.Logins.Add(login);

        context.SaveChanges();
    }
}