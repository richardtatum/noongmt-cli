namespace NoonGMT.CLI.Features.Spotify;

public class SpotifyOptions
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string AuthenticationEndpoint { get; set; }
    public required string TrackInformationEndpoint { get; set; }
}