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

        if (!string.IsNullOrWhiteSpace(options.Value.ApiKey))
        {
            _client.DefaultRequestHeaders.Add("Authorization", options.Value.ApiKey);
        }
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

    public async Task<Post?> AddAsync(Post request)
    {
        const string url = "/api/collections/posts/records";

        var response = await _client.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Post>();
    }

    public async Task<Post?> UpdateAsync(string id, Post request)
    {
        var url = $"/api/collections/posts/records/{id}";

        var response = await _client.PatchAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Post>();
    }

    public async Task<int> CountAsync()
    {
        var url = $"/api/collections/posts/records"            
            .AddQueryParameter("page", 1)
            .AddQueryParameter("perPage", 1) // We only need the total count from the body
            .AddQueryParameter("fields", "id"); // Return only the id to keep the query light

        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<GetPostsResponse>();
        return body?.TotalItems ?? 0;
    }
    
    public async Task<(int queued, DateTime? nextUp)> GetQueueAsync()
    {
        var url = $"/api/collections/posts/records"            
            .AddQueryParameter("page", 1)
            .AddQueryParameter("perPage", 100) // Its reasonable to assume there won't be over 100 post submitted in advance
            .AddQueryParameter("fields", "live_date") // Return only the live date to keep the query light
            .AddQueryParameter("skipTotal", 1)
            .AddQueryParameter("sort", "-live_date");

        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<GetPostsResponse>();

        var queued = body?.Items.Count(x => x.LiveDate > DateTime.UtcNow) ?? 0;
        var next = body?.Items
            .OrderBy(x => x.LiveDate)
            .FirstOrDefault(x => x.LiveDate > DateTime.UtcNow)
            ?.LiveDate;

        return (queued, next);
    }

    public async Task<DateTime> GetNextAvailableDateAsync()
    {
        var url = $"/api/collections/posts/records"            
            .AddQueryParameter("page", 1)
            .AddQueryParameter("perPage", 1) // We just want the most future post date.
            .AddQueryParameter("fields", "live_date")
            .AddQueryParameter("skipTotal", 1)
            .AddQueryParameter("sort", "-live_date");

        var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<GetPostsResponse>();
        var newestPost = body?.Items.FirstOrDefault()?.LiveDate;
  
        return newestPost?.AddDays(1) ?? DateTime.UtcNow.ToNoonLocalInUTC();
    }
}