namespace PodcastManager.FeedUpdater.Messages;

public record UpdatePublishedPodcast(int Code, string Title, string Feed, int CurrentErrors = 0)
    : UpdatePodcast (Code, Title, Feed, true, CurrentErrors);
    