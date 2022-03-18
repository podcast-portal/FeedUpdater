using System.Net;

namespace PodcastManager.FeedUpdater.Domain.Exceptions;

public class ServerErrorException : Exception
{
    public HttpStatusCode Code { get; }
    public string? Reason { get; }

    public ServerErrorException(HttpStatusCode code, string? reason) : base(reason)
    {
        Code = code;
        Reason = reason;
    }
}