using System.Text;
using Cocona;
using NoonGMT.CLI;
using NoonGMT.CLI.Extensions;
using NoonGMT.CLI.Features.Spotify;

var builder = CoconaApp.CreateBuilder(null, opt => { opt.EnableShellCompletionSupport = true; });

builder.Services.AddNoonGMT(builder.Configuration.GetSection(nameof(NoonGmtOptions)));
builder.Services.AddSpotify(
    builder.Configuration.GetSection(nameof(SpotifyOptions)),
    builder.Configuration.GetSection(nameof(AuthenticationOptions)));

var app = builder.Build();

app.AddCommand("list",
async ([FromService] PostService service, 
    [Option('s', Description = "The number of returned items.")] int size = 5, 
    [Option(Description = "Whether to show live and future posts.")] bool liveOnly = false, 
    [Option(Description = "Whether to include the IDs of the posts.")] bool includeIds = false,
    [Option(Description = "Whether to include the time the post goes live, along with the date.")] bool includeTime = false) =>
{
    var results = await service.GetAllAsync(size, liveOnly);
    await foreach (var post in results)
    {
        Console.WriteLine(post.ToString(includeIds, includeTime));
    }
});

app.AddCommand("add",
async ([FromService] PostService service,
    [Option('l', Description = "The date the post goes live.")] DateTime? goLiveDate,
    [Option('i', Description = "The ID or share link of the Spotify track.")] string track,
    [Option('d', Description = "An optional description.")] string? description,
    [Option(Description = "Force submit the post, even if a duplicate has been found.")] bool force = false) =>
{
    var result = await service.AddAsync(goLiveDate, track, description, force);
    if (!result.Success)
    {
        WriteErrors(result.Errors!);
        return;
    }

    Console.WriteLine("Success! New Post:");
    Console.WriteLine(result.Value!.ToString(true, true));
});

app.AddCommand("update",
async ([FromService] PostService service, 
    [Option(Description = "The id of the post.")] string? id, 
    [Option(Description = "The go live date of the post.")] DateTime? date, 
    [Option('d', Description = "The replacement post description.")] string? description,
    [Option('i', Description = "The replacement track.")] string? track,
    [Option(Description = "Force submit the post, even if a duplicate has been found.")] bool force = false) =>
{
    var result = await service.UpdateAsync(id, date, description, track, force);
    if (!result.Success)
    {
        WriteErrors(result.Errors!);
        return;
    }

    Console.WriteLine("Success! Updated post:");
    Console.WriteLine(result.Value!.ToString());
});

app.AddCommand("get", 
async ([FromService] PostService service, 
    [Option(Description = "The id of the post.")] string? id, 
    [Option(Description = "The date of the post.")] DateTime? date,
    [Option(Description = "Get the next post to release.")] bool next = false) =>
{
    // If no date is provided, or next is and its before 12, show todays release
    date ??= next && DateTime.UtcNow.Hour >= 12 ? DateTime.UtcNow.AddDays(1) : DateTime.UtcNow;
    
    var result = await service.GetAsync(id, date);
    if (!result.Success)
    {
        WriteErrors(result.Errors!);
        return;
    }
    
    if (result.Value is null)
    {
        Console.WriteLine("No post found.");
        return;
    }

    Console.WriteLine(result.Value.ToString(true, true));
});

app.AddCommand("remove",
    async ([FromService] PostService service,
        [Option(Description = "The id of the post.")] string? id, 
        [Option(Description = "The date of the post.")] DateTime? date) =>
    {
        var result = await service.RemoveAsync(id, date);
        if (!result.Success)
        {
            WriteErrors(result.Errors!);
            return;
        }
        
        Console.WriteLine("Success! Post removed:");
        Console.WriteLine(result.Value!.ToString(true, true));
    });

app.AddCommand("count", async ([FromService] PostService service) =>
{
    var total = await service.CountAsync();
    Console.WriteLine($"{total} total post(s)");
});

app.AddCommand("queue", async ([FromService] PostService service) =>
{
    var (queued, next) = await service.GetQueueAsync();
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
return;

void WriteErrors(IEnumerable<string> errors)
{
    Console.ForegroundColor = ConsoleColor.Red;
    if (!errors.Any())
    {
        Console.WriteLine("Failed but no error message provided!");
        return;
    }
    
    foreach (var error in errors)
    {
        Console.WriteLine(error);
    }
}