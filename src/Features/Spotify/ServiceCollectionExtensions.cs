using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NoonGMT.CLI.Features.Spotify;

public static class ServiceCollectionExtensions
{
    public static void AddSpotify(this IServiceCollection services, IConfigurationSection spotifyOptions, IConfigurationSection authenticationOptions)
    {
        services.Configure<SpotifyOptions>(spotifyOptions);
        services.Configure<AuthenticationOptions>(authenticationOptions);
        services.AddScoped<SpotifyClient>();
        services.AddScoped<AuthenticationService>();
        services.AddScoped<SpotifyService>();
    }
}