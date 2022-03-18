namespace PodcastManager.FeedUpdater.CrossCutting.Rabbit;

public static class RabbitConfiguration
{
    public static readonly string Url =
        Environment.GetEnvironmentVariable("RabbitUrl")
        ?? "localhost";
    public static string UpdatePodcastQueue { get; } =
        Environment.GetEnvironmentVariable("UpdatePodcast")
        ?? "PodcastManager.UpdatePodcast";
    public static string UpdatePublishedPodcastQueue { get; } =
        Environment.GetEnvironmentVariable("UpdatePublishedPodcast")
        ?? "PodcastManager.UpdatePublishedPodcast";
    public static string UpdatePodcastsQueue { get; } =
        Environment.GetEnvironmentVariable("UpdatePodcasts")
        ?? "PodcastManager.UpdatePodcasts";
    public static string UpdatePublishedPodcastsQueue { get; } =
        Environment.GetEnvironmentVariable("UpdatePublishedPodcasts")
        ?? "PodcastManager.UpdatePublishedPodcasts";
}