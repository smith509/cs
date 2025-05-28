using System;

class DateTimeArithmetic
{
    static void Main(string[] args)
    {
        // Current date and time
        DateTime now = DateTime.Now;
        Console.WriteLine($"Current Date and Time: {now}");

        // Adding days
        DateTime futureDate = now.AddDays(10);
        Console.WriteLine($"Date after adding 10 days: {futureDate}");

        // Subtracting days
        DateTime pastDate = now.AddDays(-5);
        Console.WriteLine($"Date after subtracting 5 days: {pastDate}");

        // Adding months
        DateTime futureMonthDate = now.AddMonths(2);
        Console.WriteLine($"Date after adding 2 months: {futureMonthDate}");

        // Subtracting months
        DateTime pastMonthDate = now.AddMonths(-3);
        Console.WriteLine($"Date after subtracting 3 months: {pastMonthDate}");

        // Adding years
        DateTime futureYearDate = now.AddYears(1);
        Console.WriteLine($"Date after adding 1 year: {futureYearDate}");

        // Subtracting years
        DateTime pastYearDate = now.AddYears(-2);
        Console.WriteLine($"Date after subtracting 2 years: {pastYearDate}");

        // Calculating the difference between two dates
        DateTime startDate = new DateTime(2023, 1, 1);
        DateTime endDate = new DateTime(2025, 1, 1);
        TimeSpan difference = endDate - startDate;
        Console.WriteLine($"Difference between {endDate.ToShortDateString()} and {startDate.ToShortDateString()}: {difference.Days} days");

        // Getting the total number of weeks between two dates
        double totalWeeks = difference.TotalDays / 7;
        Console.WriteLine($"Total weeks between the two dates: {totalWeeks} weeks");

        // Getting the day of the week for a specific date
        DayOfWeek dayOfWeek = now.DayOfWeek;
        Console.WriteLine($"Today is: {dayOfWeek}");
    }
}
