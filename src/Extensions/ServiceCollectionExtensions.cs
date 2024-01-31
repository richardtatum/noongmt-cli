using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NoonGMT.CLI.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddNoonGMT(this IServiceCollection services, IConfigurationSection noonGmtOptions)
    {
        services.AddScoped<PostClient>();
        services.AddScoped<PostService>();
        services.Configure<NoonGmtOptions>(noonGmtOptions);
    }
}