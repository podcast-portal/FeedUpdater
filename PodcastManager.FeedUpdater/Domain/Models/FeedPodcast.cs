namespace PodcastManager.FeedUpdater.Domain.Models;

public record FeedPodcast(
    string Title,
    string? Link = null,
    string? Description = null,
    string? Language = null,
    Image? Image = null,
    string? Subtitle = null,
    string? Summary = null,
    Owner? Owner = null);