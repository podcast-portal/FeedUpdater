namespace PodcastManager.FeedUpdater.Domain.Exceptions;

public class NumberBadFormattedException : Exception
{
    public string? Text { get; }

    public NumberBadFormattedException(string? text)
    {
        Text = text;
    }
}