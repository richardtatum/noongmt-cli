using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using NoonGMT.CLI.Extensions;
using NoonGMT.CLI.Models;

namespace NoonGMT.CLI;

public class PostClient
{
    private readonly HttpClient _client;

    public PostClient(IOptions<NoonGmtOptions> options)
    {
        _client = new()
        {
            BaseAddress = new Uri(options.Value.BaseUrl),
        };
        
        _client.DefaultRequestHeaders.Add("Authorization", options.Value.ApiKey);
    }

    public async Task<Post?> GetAsync(string id)
    {
        var url = $"/api/collections/posts/records/{id}";

        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Post>();
    }
    
    public async Task<Post?> GetAsync(DateTime date)
    {
        var dateString = date.ToNoonLocalInUTC().ToDateString();
        
        var url = "/api/collections/posts/records"
            .AddQueryParameter("page", 1)
            .AddQueryParameter("perPage", 1)
            .AddQueryParameter("skipTotal", 1)
            .AddQueryParameter("filter", $"live_date='{dateString}'");
        
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<GetPostsResponse>();

        return body?.Items.FirstOrDefault() ?? null;
    }

    public async Task<Post[]> GetAllAsync(int listSize = 5, bool showOnlyLive = true)
    {
        var url = "/api/collections/posts/records"
            .AddQueryParameter("page", 1)
            .AddQueryParameter("perPage", listSize)
            .AddQueryParameter("skipTotal", 1)
            .AddQueryParameter("sort", "-live_date");

        if (showOnlyLive)
        {
            url = url.AddQueryParameter("filter", "live_date<=@now");
        }
        
        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<GetPostsResponse>();

        return body?.Items ?? Array.Empty<Post>();
    }

    public async Task<string?> AddAsync(Post request)
    {
        const string url = "/api/collections/posts/records";

        var response = await _client.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<Post>();

        return body?.Id;
    }

    public async Task<Post?> UpdateAsync(string id, Post request)
    {
        var url = $"/api/collections/posts/records/{id}";

        var response = await _client.PatchAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Post>();
    }

    public async Task<(int total, int queued, DateTime? nextUp)> CountAsync()
    {
        var url = $"/api/collections/posts/records"            
            .AddQueryParameter("page", 1)
            .AddQueryParameter("perPage", 100) // Its reasonable to assume there won't be over 100 post submitted in advance
            .AddQueryParameter("fields", "live_date") // Return only the live date to keep the query light
            .AddQueryParameter("sort", "-live_date");

        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<GetPostsResponse>();

        var total = body.TotalItems;
        var queued = body.Items.Count(x => x.LiveDate > DateTime.UtcNow);
        var next = body.Items
            .OrderBy(x => x.LiveDate)
            .FirstOrDefault(x => x.LiveDate > DateTime.UtcNow)
            ?.LiveDate;

        return (total, queued, next);
    }
}