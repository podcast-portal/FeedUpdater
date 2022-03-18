namespace PodcastManager.FeedUpdater.Domain.Exceptions;

public class DateTimeBadFormattedException : Exception
{
    public string BadFormattedDateTime { get; }

    public DateTimeBadFormattedException(string badFormattedDateTime)
    {
        BadFormattedDateTime = badFormattedDateTime;
    }
}