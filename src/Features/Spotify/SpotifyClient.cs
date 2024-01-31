using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using NoonGMT.CLI.Extensions;

namespace NoonGMT.CLI.Features.Spotify;

public class SpotifyClient(IOptions<SpotifyOptions> options)
{
    private readonly HttpClient _client = new();

    public async Task<AuthenticationInformation?> AuthenticateAsync()
    {
        var basicToken = Base64Encode($"{options.Value.ClientId}:{options.Value.ClientSecret}");
        var url = options.Value.AuthenticationEndpoint;

        var request = new HttpRequestMessage(HttpMethod.Post, url)
            .AddBasicAuthorization(basicToken)
            .AddFormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new("grant_type", "client_credentials")
            });

        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<AuthenticationInformation>();
    }

    public async Task<TrackInformation?> GetTrackInformationAsync(string bearerToken, string trackId)
    {
        var url = $"{options.Value.TrackInformationEndpoint}/{trackId}";

        var request = new HttpRequestMessage(HttpMethod.Get, url)
            .AddBearerToken(bearerToken);

        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TrackInformation>();
    }

    private static string Base64Encode(string value)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }
}