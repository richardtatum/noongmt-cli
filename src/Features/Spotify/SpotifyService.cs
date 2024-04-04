namespace NoonGMT.CLI.Features.Spotify;

public class SpotifyService(AuthenticationService authService, SpotifyClient spotifyClient)
{
    private string? BearerToken { get; set; }
    
    public async Task<string?> GetTrackSummaryAsync(string trackId)
    {
        var bearerToken = BearerToken ??= await authService.GetBearerTokenAsync();
        if (bearerToken is null)
        {
            // No bearer token means we can't obtain track information.
            return null;
        }

        var response = await spotifyClient.GetTrackInformationAsync(bearerToken, trackId);
        return response?.ToString();
    }
}