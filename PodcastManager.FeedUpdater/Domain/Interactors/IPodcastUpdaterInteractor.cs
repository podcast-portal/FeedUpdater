using PodcastManager.FeedUpdater.Messages;

namespace PodcastManager.FeedUpdater.Domain.Interactors;

public interface IPodcastUpdaterInteractor
{
    Task Execute(UpdatePodcast podcast);
}