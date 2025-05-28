using System;

class DateConversion
{
    // Convert DateTime to string
    public static string DateTimeToString(DateTime date)
    {
        return date.ToString("yyyy-MM-dd"); // Format as "YYYY-MM-DD"
    }

    // Convert string to DateTime
    public static DateTime StringToDateTime(string dateString)
    {
        return DateTime.Parse(dateString);
    }

    // Convert DateTime to int (year)
    public static int DateTimeToInt(DateTime date)
    {
        return date.Year;
    }

    // Convert int (year) to DateTime (assuming January 1st of that year)
    public static DateTime IntToDateTime(int year)
    {
        return new DateTime(year, 1, 1);
    }

    // Convert int (year) to string
    public static string IntToString(int year)
    {
        return year.ToString();
    }

    // Convert string to int (year)
    public static int StringToInt(string yearString)
    {
        return int.Parse(yearString);
    }

    // Convert DateTime to int (yyyymmdd)
    public static int DateTimeToIntYyyyMmDd(DateTime date)
    {
        return int.Parse(date.ToString("yyyyMMdd")); // Format as "YYYYMMDD"
    }

    // Convert int (yyyymmdd) to DateTime
    public static DateTime IntYyyyMmDdToDateTime(int yyyymmdd)
    {
        string dateString = yyyymmdd.ToString();
        int year = int.Parse(dateString.Substring(0, 4));
        int month = int.Parse(dateString.Substring(4, 2));
        int day = int.Parse(dateString.Substring(6, 2));
        return new DateTime(year, month, day);
    }

    static void Main(string[] args)
    {
        // Example usage
        DateTime now = DateTime.Now;

        // DateTime to string
        string dateString = DateTimeToString(now);
        Console.WriteLine($"DateTime to String: {dateString}");

        // String to DateTime
        DateTime parsedDate = StringToDateTime(dateString);
        Console.WriteLine($"String to DateTime: {parsedDate}");

        // DateTime to int (year)
        int year = DateTimeToInt(now);
        Console.WriteLine($"DateTime to Int (Year): {year}");

        // Int (year) to DateTime
        DateTime yearDate = IntToDateTime(year);
        Console.WriteLine($"Int (Year) to DateTime: {yearDate}");

        // Int (year) to string
        string yearString = IntToString(year);
        Console.WriteLine($"Int (Year) to String: {yearString}");

        // String to int (year)
        int parsedYear = StringToInt(yearString);
        Console.WriteLine($"String to Int (Year): {parsedYear}");

        // DateTime to int (yyyymmdd)
        int yyyymmdd = DateTimeToIntYyyyMmDd(now);
        Console.WriteLine($"DateTime to Int (YYYYMMDD): {yyyymmdd}");

        // Int (yyyymmdd) to DateTime
        DateTime dateFromInt = IntYyyyMmDdToDateTime(yyyymmdd);
        Console.WriteLine($"Int (YYYYMMDD) to DateTime: {dateFromInt}");
    }
}
