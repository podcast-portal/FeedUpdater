namespace PodcastManager.FeedUpdater.Domain.Models;

public record Feed(
    string Title,
    string? Link = null,
    string? Description = null,
    string? Language = null,
    Image? Image = null,
    string? Subtitle = null,
    string? Summary = null,
    Owner? Owner = null,
    bool IsEmpty = false)
{
    public string[] Categories { get; init; } = Array.Empty<string>();
    public Item[] Items { get; init; } = Array.Empty<Item>();
    
    public static Feed Empty() => new(string.Empty, string.Empty, IsEmpty: true);
}