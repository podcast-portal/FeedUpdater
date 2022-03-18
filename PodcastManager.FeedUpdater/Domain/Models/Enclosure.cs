namespace PodcastManager.FeedUpdater.Domain.Models;

public record Enclosure(string Url, long Length, string Type);