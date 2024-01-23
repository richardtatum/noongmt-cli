using System.Globalization;

namespace NoonGMT.CLI.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToNoonLocal(this DateTime date) =>
        new(DateOnly.FromDateTime(date), new TimeOnly(12, 00, 00), DateTimeKind.Local);

    public static DateTime ToNoonLocalInUTC(this DateTime date) => ToNoonLocal(date).ToUniversalTime();
    
    public static string? ToDateString(this DateTime date) => date.ToString("yyyy-MM-dd HH:mm:ss.fffZ");

}