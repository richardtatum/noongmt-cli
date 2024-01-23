namespace NoonGMT.CLI.Models;

public class GetPostsResponse
{
    public int TotalItems { get; set; }
    public Post[] Items { get; set; } = Array.Empty<Post>();
}