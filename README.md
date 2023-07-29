# RMIT - WDT - SP2 - A2
Name: Carelle Mulawa-Richards<br>
Student ID: s3749114<br>
Repository: https://github.com/rmit-wdt-sp2-2023/s3749114-a2

## Usage notes

## Web API documentation

### Customer endpoints

**GET** /customer
Return value: List<Customer>
Description: Retrieves all Customers.

**GET** /customer{id}
Return value: Customer
Description: Retrieves a Customer with the given CustomerID.

**PUT** /customer{customer}
Return value: None
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
| ProfilePicture  | String | Picture file name |

### Login endpoints

**GET** /login
Return value: List<Login>
Description: Retrieves all Logins.

**GET** /login/{id}
Return value: Login
Description: Retrieves a Logins with the given LoginID.

**PUT** /login/{login}
Return value: None
Description: Update the details of the given Login.

**Login**

| Name  | Data type | Description |
| ------------- | ------------- | ------------- |
| LoginID | string | Must be 8 digits |
| CustomerID  | int | Must be 4 digits |
| PasswordHash  | string | Max 94 chars |
| LoginStatus  | LoginStatus | Enum, locked = 1 and unlocked = 2 |

### BillPay endpoints

**GET** /billpay
Return value: List<BillPay>
Description: Retrieves all BillPays.

**GET** /billpay/{id}
Return value: BillPay
Description: Retrieves a BillPay with the given BillPayID.

**PUT** /billpay/{billPay}
Return value: None
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