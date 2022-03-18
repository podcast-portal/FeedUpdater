namespace PodcastManager.FeedUpdater.Domain.Models;

public record FullPodcast(
        PodcastStatus? Status,
        ImportedPodcast Imported,
        int Code,
        string Title,
        string Feed,
        bool IsPublished = false);