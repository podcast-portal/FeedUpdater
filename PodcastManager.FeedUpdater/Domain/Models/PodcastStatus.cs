namespace PodcastManager.FeedUpdater.Domain.Models;

public record PodcastStatus(
    DateTime NextUpdate,
    long? TotalEpisodes = 0,
    DateTime? LastTimeUpdated = null,
    int ErrorCount = 0,
    string[]? Errors = null
);