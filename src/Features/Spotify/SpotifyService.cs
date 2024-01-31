namespace NoonGMT.CLI.Features.Spotify;

public class SpotifyService(AuthenticationService authService, SpotifyClient spotifyClient)
{
    private string? BearerToken { get; set; }
    
    public async Task<string?> GetTrackSummaryAsync(string trackId)
    {
        var bearerToken = BearerToken ??= await authService.GetBearerTokenAsync();
        var response = await spotifyClient.GetTrackInformationAsync(bearerToken, trackId);
        return response?.ToString();
    }
}