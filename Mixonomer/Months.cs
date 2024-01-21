namespace Mixonomer;

public static class Months
{
    public static string ThisMonth() => DateTime.Now.ToString("MMMM yy").ToLowerInvariant();

    public static string LastMonth()
    {
        var now = DateTime.Now;
        var lastMonth = now.AddDays(-now.Day - 1);
        return lastMonth.ToString("MMMM yy").ToLowerInvariant();
    }
}