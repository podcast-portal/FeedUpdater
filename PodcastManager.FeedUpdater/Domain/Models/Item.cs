namespace PodcastManager.FeedUpdater.Domain.Models;

public record Item(
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
    Image? Image = null,
    bool IsEmpty = false)
{
    public static Item Empty()
    {
        return new Item(string.Empty,
            DateTime.MinValue,
            string.Empty,
            new Enclosure(string.Empty, 0, string.Empty),
            IsEmpty: true);
    }
}