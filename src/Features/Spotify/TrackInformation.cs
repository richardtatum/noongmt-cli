namespace NoonGMT.CLI.Features.Spotify;

public class TrackInformation
{
    public Artist[] Artists { get; set; } = Array.Empty<Artist>();
    public string? Name { get; set; }

    public override string ToString()
    {
        var artists = string.Join(", ", Artists.Select(x => x.Name));
        return $"{artists} - {Name}";
    }
}

public class Artist
{
    public string? Name { get; set; }
}