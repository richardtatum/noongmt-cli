using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using NoonGMT.CLI.Extensions;

namespace NoonGMT.CLI.Features.Spotify;

public class SpotifyClient
{
    private readonly HttpClient _client;
    private readonly IOptions<SpotifyOptions> _options;

    public SpotifyClient(IOptions<SpotifyOptions> options)
    {
        _options = options;
        _client = new();
    }

    public async Task<AuthenticationInformation> AuthenticateAsync()
    {
        var basicToken = Base64Encode($"{_options.Value.ClientId}:{_options.Value.ClientSecret}");
        var url = _options.Value.AuthenticationEndpoint;

        var request = new HttpRequestMessage(HttpMethod.Post, url)
            .AddBasicAuthorization(basicToken)
            .AddFormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("grant_type", "client_credentials")
            });

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AuthenticationInformation>();
    }

    public async Task<TrackInformation> GetTrackInformationAsync(string bearerToken, string trackId)
    {
        var url = $"{_options.Value.TrackInformationEndpoint}/{trackId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .AddBearerToken(bearerToken);

        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TrackInformation>();
    }

    private static string Base64Encode(string value)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }
}