# RMIT - WDT - SP2 - A2
Name: Carelle Mulawa-Richards<br>
Student ID: s3749114<br>
Repository: https://github.com/rmit-wdt-sp2-2023/s3749114-a2

Description: An ASP.NET Core MVC Website for internet banking with .NET 7.0 written in C# with an Azure Microsoft SQL Server database. Database access is implemented with EF Core as the ORM and no SQL statements are present.

Assignment completion: I have attempted to complete all parts of the assignment except for unit testing. Some unit tests have been written but these do not cover the entire program. 

## Usage notes

If you would like to open all my projects in Visual Studio, you can use the **BankApplication.sln**.

The commands below assume the user is in the root directory, i.e., the **s3749114-a2** folder. These commands are for Terminal on Mac OS. You may need to adapt these for your OS.

### Customer application

The customer website is located in the **CustomerApplication** folder. It is responsible for seeding data, so it should be run first. To run, you can use:
```
cd CustomerApplication

dotnet run
```
Once the application is running, open a web browser and go to: http://localhost:5210

Note that my migrations are in the **BankLibrary** folder. To add migrations and update the database, I used:
```
cd CustomerApplication

dotnet ef migrations add MigrationName --project ../BankLibrary/BankLibrary.csproj --startup-project CustomerApplication.csproj   

dotnet ef database update   
```
Two Payees have been seeded to the database. You can use Payee ID "1" or "2" when scheduling a BillPay.

I have used service classes to handle logic relating to database and models. These are used by the controllers. You can find these services in the **Services** folder. The background service for BillPay is in the **BackgroundServices** folder.  

### Web API

The adminwWeb API is located in the **WebApi** folder. This can be run using:
```
cd WebApi

dotnet run
```

### Admin application

The admin portal website is located in the **AdminPortal** folder. Note that the web API must be running before the admin application is run. To run the admin application, use:
```
cd AdminPortal

dotnet run
```
Once the application is running, open a web browser and go to: http://localhost:5016

To log in, use: username = admin, password = admin

### Bank library

I have put my models and DBContext in the **BankLibrary** folder. This is so the customer and admin applcation can use these. The **Validation** folder contains some of the validation methods for data annoations. 

### Unit tests

Tests are located in the **BankLibrary.Tests** and **CustomerApplication.Tests** folders. You can run these with: 
```
cd BankLibrary.Tests

dotnet test
```
```
cd CustomerApplication.Tests

dotnet test
```

## Web API documentation

### Customer endpoints

**GET** /customer<br>
Return value: List<Customer><br>
Description: Retrieves all Customers.

**GET** /customer{id}<br>
Return value: Customer<br>
Description: Retrieves a Customer with the given CustomerID.

**PUT** /customer{customer}<br>
Return value: None<br>
Description: Update the details of the given Customer.

**Customer**

| Name  | Data type | Description |
| ------------- | ------------- | ------------- |
| CustomerID  | int | Must be 4 digits |
| Name  | string | Max 30 chars |
| TFN  | string | Must be digits in the format of: XXX XXX XXX |
| Address | string | Max 30 chars |
| City  | string | Max 40 chars |
| State  | string | A 2 or 3 lettered Australian state |
| PostCode  | string | Must be 4 digits |
| Mobile  | string | Must be digits in the format of: 04XX XXX XXX |

### Login endpoints

**GET** /login<br>
Return value: List<Login><br>
Description: Retrieves all Logins.

**GET** /login/{id}<br>
Return value: Login<br>
Description: Retrieves a Logins with the given LoginID.

**PUT** /login/{login}<br>
Return value: None<br>
Description: Update the details of the given Login.

**Login**

| Name  | Data type | Description |
| ------------- | ------------- | ------------- |
| LoginID | string | Must be 8 digits |
| CustomerID  | int | Must be 4 digits |
| PasswordHash  | string | Max 94 chars |
| LoginStatus  | LoginStatus | Enum, locked = 1 and unlocked = 2 |

### BillPay endpoints

**GET** /billpay<br>
Return value: List<BillPay><br>
Description: Retrieves all BillPays.

**GET** /billpay/{id}<br>
Return value: BillPay<br>
Description: Retrieves a BillPay with the given BillPayID.

**PUT** /billpay/{billPay}<br>
Return value: None<br>
Description: Update the details of the given BillPay.

**BillPay**

| Name  | Data type | Description |
| ------------- | ------------- | ------------- |
| BillPayID  | int |  |
| AccountNumber  | int | Must be 4 digits |
| PayeeID  | int |  |
| Amount | decimal | Must be less than 3 decimals and cannot be negative |
| ScheduleTimeUtc  | DateTime |  |
| Period  | Period | Enum,  OneOff = 1, Failed = 2 and Monthly = 3  |
| BillPayStatus  | BillPayStatus | Enum,  Blocked = 1, Failed = 2 and Scheduled = 3  |

## References

_Please note that no references were used for writing this document. This list is for citations made within my code._

Bolger M (2023) ‘Class A Webinar / recording_6’ [Recorded webinar], RMIT University, Melbourne, accessed 29 July 2023. https://rmit.instructure.com/courses/112872/external_tools/546 (requires login)

DragonImages (2021) Checking banking account, Envato Elements, accessed 29 July 2021. https://elements.envato.com/checking-banking-account-9SBNGLT