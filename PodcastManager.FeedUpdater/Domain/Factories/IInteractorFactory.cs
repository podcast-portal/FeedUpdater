using PodcastManager.FeedUpdater.Domain.Interactors;

namespace PodcastManager.FeedUpdater.Domain.Factories;

public interface IInteractorFactory
{
    IMultiplePodcastUpdaterInteractor CreateMultiple();
    IPodcastUpdaterInteractor CreatePodcastUpdater();
}