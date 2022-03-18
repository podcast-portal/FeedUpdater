namespace PodcastManager.FeedUpdater.Messages;

public record UpdatePodcast(int Code, string Title, string Feed, bool IsPublished = false, int CurrentErrors = 0);