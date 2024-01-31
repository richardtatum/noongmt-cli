namespace NoonGMT.CLI.Features.Spotify;

public class SpotifyService
{
    private readonly AuthenticationService _authService;
    private readonly SpotifyClient _spotifyClient;

    public SpotifyService(AuthenticationService authService, SpotifyClient spotifyClient)
    {
        _authService = authService;
        _spotifyClient = spotifyClient;
    }

    public async Task<string> GetTrackSummaryAsync(string trackId)
    {
        var bearerToken = await _authService.GetBearerTokenAsync();
        var response = await _spotifyClient.GetTrackInformationAsync(bearerToken, trackId);
        return response.ToString();
    }
}