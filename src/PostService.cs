using NoonGMT.CLI.Extensions;
using NoonGMT.CLI.Features.Spotify;
using NoonGMT.CLI.Models;

namespace NoonGMT.CLI;

public class PostService(PostClient client, SpotifyService spotifyService)
{
    public async Task<Post[]> GetAllAsync(int size, bool liveOnly)
    {
        var results = await client.GetAllAsync(size, liveOnly);

        foreach (var post in results)
        {
            post.TrackSummary = await spotifyService.GetTrackSummaryAsync(post.TrackId);
        }

        return results;
    }

    public async Task<Result<Post>> AddAsync(DateTime? goLiveDate, string track, string? description, bool force = false)
    {
        var date = goLiveDate?.ToNoonLocalInUTC() ?? await client.GetNextAvailableDateAsync();
        
        // We only need to check the date is free if its provided
        if (goLiveDate is not null)
        {
            var existingPost = await client.GetAsync(date);
            if (existingPost is not null)
            {
                return new Result<Post>(new[]
                    { "Post already exists for this date. Original post:", existingPost.ToString(true) });
            }
        }

        var trackId = track.GetTrackId();
        var duplicate = await client.GetByTrackIdAsync(trackId);
        if (duplicate is not null && !force)
        {
            return new Result<Post>(new[]
                { "Track has already been submitted. Original post:", duplicate.ToString(true) });
        }
        
        var post = new Post
        {
            LiveDate = date,
            TrackId = trackId,
            Description = description
        };

        var response = await client.AddAsync(post);

        return new Result<Post>(response!);
    }

    public async Task<Result<Post>> UpdateAsync(string? id, DateTime? date, string? description, string? track,
        bool force = false)
    {
        if (string.IsNullOrWhiteSpace(id) && date is null)
        {
            return new Result<Post>("Id or Date need to be provided.");
        }
        
        var existingPost = !string.IsNullOrWhiteSpace(id)
            ? await client.GetAsync(id)
            : await client.GetAsync(date!.Value);

        if (existingPost is null)
        {
            return new Result<Post>("Id or Date need to be provided.");
        }
        
        var trackId = track?.GetTrackId();
        if (trackId is not null)
        {
            var duplicate = await client.GetByTrackIdAsync(trackId);
            if (duplicate is not null && !force)
            {
                return new Result<Post>(new[]
                    { "Track has already been submitted. Original post:", duplicate.ToString(true) });
            }
        }
        
        existingPost.TrackId = trackId ?? existingPost.TrackId;
        existingPost.Description = description ?? existingPost.Description;

        var result = await client.UpdateAsync(existingPost.Id!, existingPost);

        return new Result<Post>(result!);
    }

    public async Task<Result<Post>> GetAsync(string? id, DateTime? date)
    {
        if (string.IsNullOrWhiteSpace(id) && date is null)
        {
            return new Result<Post>("Id or Date need to be provided.");
        }

        var result = !string.IsNullOrWhiteSpace(id)
            ? await client.GetAsync(id)
            : await client.GetAsync(date!.Value);

        return new Result<Post>(result);
    }

    public Task<int> CountAsync() => client.CountAsync();

    public Task<(int queued, DateTime? next)> GetQueueAsync() => client.GetQueueAsync();
}