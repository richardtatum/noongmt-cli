namespace NoonGMT.CLI.Models;

public class Result<T>
{
    public Result() { }

    public Result(string error)
    {
        Success = false;
        Errors = new []{ error };
    }
    
    public Result(IEnumerable<string> errors)
    {
        Success = false;
        Errors = errors;
    }

    public Result(T? value)
    {
        Success = true;
        Value = value;
    }

    public bool Success { get; set; }
    public T? Value { get; set; }
    public IEnumerable<string>? Errors { get; set; } = Array.Empty<string>();
}