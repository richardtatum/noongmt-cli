using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;

namespace NoonGMT.CLI.Extensions;

public static class StringExtensions
{
    public static string AddQueryParameter(this string uri, string name, object? value)
        => QueryHelpers.AddQueryString(uri, name, value?.ToString() ?? string.Empty);
    
    public static DateTime FromDateString(this string date)
        => DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss.fffZ", CultureInfo.InvariantCulture);

    public static string GetTrackId(this string value)
    {
        // If it is a URI its a share link.
        // Format is https://open.spotify.com/track/5fBjPtOxhgdpU6LNWLyVHv
        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            Console.WriteLine("Detected share link. Extracting track ID.");
            return uri.Segments.Last();
        }

        return value;
    }

}