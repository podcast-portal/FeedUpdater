namespace PodcastManager.FeedUpdater.Domain.Models;

public record Episode(
    int PodcastCode,
    string Title,
    DateTime PublicationDate,
    string Guid,
    Enclosure Enclosure,
    string? Link = null,
    string? Description = null,
    string? Subtitle = null,
    string? Summary = null,
    string? Author = null,
    TimeSpan? Duration = null,
    Image? Image = null);