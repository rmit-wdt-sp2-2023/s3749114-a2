namespace CustomerApplication.Utilities.DisplayUtilities;

public static class DisplayUtilities
{
    public static string UtcToLocal(DateTime time) =>
        TimeZoneInfo.ConvertTimeFromUtc(time, TimeZoneInfo.Local).ToString("dd/MM/yyyy hh:mm tt").ToUpper();

    //public static DateTime UtcToLocal(DateTime time)
    //{
    //    TimeZoneInfo timeZone = TimeZoneInfo.Local;


    //}

}

