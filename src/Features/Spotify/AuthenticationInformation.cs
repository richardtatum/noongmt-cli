using System.Text.Json.Serialization;

namespace NoonGMT.CLI.Features.Spotify;

public class AuthenticationInformation
{
    [JsonIgnore]
    private DateTime? _expires;

    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; set; }

    public DateTime Expires
    {
        get => _expires ?? DateTime.UtcNow.AddSeconds(ExpiresIn);
        set => _expires = value;
    }
}
