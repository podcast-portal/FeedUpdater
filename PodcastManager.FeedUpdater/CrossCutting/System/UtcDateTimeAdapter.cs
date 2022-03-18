using PodcastManager.FeedUpdater.Adapters;

namespace PodcastManager.FeedUpdater.CrossCutting.System;

public class UtcDateTimeAdapter : IDateTimeAdapter
{
    public DateTime Now() => DateTime.UtcNow;
}