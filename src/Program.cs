using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Cocona;
using NoonGMT.CLI;
using NoonGMT.CLI.Extensions;
using NoonGMT.CLI.Models;

var builder = CoconaApp.CreateBuilder(null, opt =>
{
    opt.EnableShellCompletionSupport = true;
});

builder.Services.AddScoped<PostClient>();
builder.Services.Configure<NoonGmtOptions>(builder.Configuration.GetSection(nameof(NoonGmtOptions)));

var app = builder.Build();

app.AddCommand("list",
    async ([FromService] PostClient client, 
        [Option('s', Description = "The number of returned items.")] int size = 5, 
        [Option(Description = "Whether to show live and future posts.")] bool showOnlyLive = false, 
        [Option(Description = "Whether to include the IDs of the posts.")] bool includeIds = false,
        [Option(Description = "Whether to include the time the post goes live, along with the date.")] bool includeTime = false) =>
    {
        var results = await client.GetAllAsync(size, showOnlyLive);
        foreach (var post in results.OrderByDescending(x => x.LiveDate))
        {
            Console.WriteLine(post.ToString(includeIds, includeTime));
        }
    });

    var date = goLiveDate.ToNoonLocalInUTC();
    var existingPost = await client.GetAsync(date);
    if (existingPost is not null)
    {
        Console.WriteLine("Post already exists for this date.");
        return;
    }
    
    var post = new Post
    {
        LiveDate = date,
        TrackId = trackId,
        Description = description
    };
    
    var result = await client.AddAsync(post);
    
    Console.WriteLine($"Success! New ID: {result}");
});
app.AddCommand("add",
    async ([FromService] PostClient client,
        [Option('l', Description = "The date the post goes live.")] DateTime goLiveDate,
        [Option('i', Description = "The ID or share link of the Spotify track.")] string track,
        [Option('d', Description = "An optional description.")] string? description) =>
    {

    if (string.IsNullOrWhiteSpace(id) && date is null)
    {
        Console.WriteLine("Id or Date need to be provided.");
        return;
    }
    
    var existingPost = !string.IsNullOrWhiteSpace(id) 
        ? await client.GetAsync(id) 
        : await client.GetAsync(date!.Value);
    
    if (existingPost is null)
    {
        Console.WriteLine("No post found.");
        return;
    }
app.AddCommand("update",
    async ([FromService] PostClient client, 
        [Option(Description = "The id of the post.")] string? id, 
        [Option(Description = "The go live date of the post.")] DateTime? date, 
        [Option('d', Description = "The replacement post description.")] string? description,
        [Option('i', Description = "The replacement track.")] string? track) =>
    {

    existingPost.TrackId = trackId ?? existingPost.TrackId;
    existingPost.Description = description ?? existingPost.Description;

    var result = await client.UpdateAsync(existingPost.Id!, existingPost);
    
    Console.WriteLine("Success! Updated post:");
    Console.WriteLine(result!.ToString());
});

app.AddCommand("get", 
    async ([FromService] PostClient client, 
        [Option(Description = "The id of the post.")] string? id, 
        [Option(Description = "The date of the post.")] DateTime? date) =>
{
    if (string.IsNullOrWhiteSpace(id) && date is null)
    {
        Console.WriteLine("Id or Date need to be provided.");
        return;
    }

    var result = !string.IsNullOrWhiteSpace(id) 
        ? await client.GetAsync(id) 
        : await client.GetAsync(date!.Value);
    
    if (result is null)
    {
        Console.WriteLine("No post found.");
        return;
    }

    Console.WriteLine(result.ToString(true));
});

app.AddCommand("count", async ([FromService] PostClient client) =>
{
    var total = await client.CountAsync();
    Console.WriteLine($"{total} total post(s)");
});

app.AddCommand("queue", async ([FromService] PostClient client) =>
{
    var (queued, next) = await client.GetQueueAsync();
    var responseBuilder = new StringBuilder($"{queued} post(s) queued up for publishing.");
    if (next is not null)
    {
        responseBuilder.Append($" Next post is due {next.Value.ToShortDateString()}");
    }

    Console.ForegroundColor = queued switch
    {
        > 2 => ConsoleColor.Green,
        2 => ConsoleColor.DarkYellow,
        < 2 => ConsoleColor.Red
    };

    Console.WriteLine(responseBuilder.ToString());
    Console.ResetColor();
});

app.Run();