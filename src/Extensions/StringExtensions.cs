using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;

namespace NoonGMT.CLI.Extensions;

public static class StringExtensions
{
    public static string AddQueryParameter(this string uri, string name, object? value)
        => QueryHelpers.AddQueryString(uri, name, value?.ToString() ?? string.Empty);
    
    public static DateTime FromDateString(this string date) => DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss.fffZ", CultureInfo.InvariantCulture);

}