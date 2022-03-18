namespace PodcastManager.FeedUpdater;

public static class FeedUpdaterConfiguration
{
    public static readonly TimeSpan PublishedPodcastNextSchedule =
        TimeSpan.Parse(Environment.GetEnvironmentVariable("PublishedPodcastNextSchedule")
                       ?? TimeSpan.FromMinutes(30).ToString());
    public static readonly TimeSpan PodcastNextSchedule =
        TimeSpan.Parse(Environment.GetEnvironmentVariable("PodcastNextSchedule")
                       ?? TimeSpan.FromHours(30).ToString());
}