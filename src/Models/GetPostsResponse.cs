namespace NoonGMT.CLI.Models;

public class GetPostsResponse
{
    public int Page { get; set; }
    public int PerPage { get; set; }
    public Post[] Items { get; set; } = Array.Empty<Post>();
}