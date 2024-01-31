using System.Text.Json;
using Microsoft.Extensions.Options;

namespace NoonGMT.CLI.Features.Spotify;

public class AuthenticationService(SpotifyClient client, IOptions<AuthenticationOptions> options)
{
    public async Task<string> GetBearerTokenAsync()
    {
        var filePath = options.Value.FilePath;
        
        // If a store already exists, try to use that first
        if (File.Exists(filePath))
        {
            var store = await File.ReadAllTextAsync(filePath);
            var auth = JsonSerializer.Deserialize<AuthenticationInformation>(store);
            if (auth?.Expires >= DateTime.UtcNow)
            {
                return auth.AccessToken;
            }
        }

        // Otherwise auth with the client and store to file
        var response = await client.AuthenticateAsync();
        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, response);
        
        return response.AccessToken;
    }
    
}