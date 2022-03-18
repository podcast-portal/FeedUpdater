using PodcastManager.FeedUpdater.Domain.Repositories;

namespace PodcastManager.FeedUpdater.Domain.Factories;

public interface IRepositoryFactory
{
    IEpisodeRepository CreateEpisode();
    IPodcastRepository CreatePodcast();
}