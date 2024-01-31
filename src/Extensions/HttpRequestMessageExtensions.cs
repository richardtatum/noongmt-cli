namespace NoonGMT.CLI.Extensions;

public static class HttpRequestMessageExtensions
{
    public static HttpRequestMessage AddBasicAuthorization(this HttpRequestMessage message, string token) =>
        message.AddAuthHeader($"Basic {token}");

    public static HttpRequestMessage AddBearerToken(this HttpRequestMessage message, string token) =>
        message.AddAuthHeader($"Bearer {token}");

    public static HttpRequestMessage AddFormUrlEncodedContent(this HttpRequestMessage message,
        IEnumerable<KeyValuePair<string, string>> value)
    {
        message.Content = new FormUrlEncodedContent(value);
        return message;
    }

    private static HttpRequestMessage AddAuthHeader(this HttpRequestMessage message, string value)
    {
        message.Headers.Add("Authorization", value);
        return message;
    }
}