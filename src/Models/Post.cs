using System.Text.Json.Serialization;
using  NoonGMT.CLI.Extensions;

namespace NoonGMT.CLI.Models;

public class Post
{
    private string? _liveDateString;
    private DateTime? _liveDate;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; set; }

    [JsonPropertyName("track_id")]
    public string TrackId { get; set; } = null!;
    public string? Description { get; set; }

    [JsonPropertyName("live_date")]
    public string? LiveDateString
    {
        get => _liveDateString ?? LiveDate?.ToDateString();
        init => _liveDateString = value;
    }

    public DateTime? LiveDate
    {
        get => _liveDate ?? LiveDateString?.FromDateString();
        init => _liveDate = value;
    }

    public string ToString(bool includeId = false, bool includeTime = false)
    {
        var date = includeTime
            ? $"[{LiveDate?.ToString()}] "
            : $"[{LiveDate?.ToShortDateString()}] ";
        
        var builder = new StringBuilder(date);
        if (includeId)
        {
            builder.Append($"Id: {Id}, ");
        }

        builder.Append($"Track: {TrackId}, Description: {Description}");

        return builder.ToString();
    }

}