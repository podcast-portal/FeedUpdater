namespace PodcastManager.FeedUpdater.Domain.Models;

public record ImportedPodcast(
    FeedPodcast? Feed = null
);