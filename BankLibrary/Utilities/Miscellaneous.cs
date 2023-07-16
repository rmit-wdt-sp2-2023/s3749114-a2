namespace BankLibrary.Utilities.Miscellaneous;

public static class Miscellaneous
{
    // Takes a UTC time and converts it to the user's local time. 

    public static string UtcToLocal(DateTime utcTime) =>
        TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local).ToString("dd/MM/yyyy hh:mm tt").ToUpper();
}