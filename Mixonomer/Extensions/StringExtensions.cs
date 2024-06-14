namespace Mixonomer.Extensions;

public static class StringExtensions
{
    public static string UriToId(this string uri) => uri.Split(':')[2];
}