namespace NoonGMT.CLI.Features.Spotify;

public class AuthenticationOptions
{
    public required string FilePath { get; set; } = "spotify-auth.json";
    public required int ExpiryThreshold { get; set; } = 600;
}