using PodcastManager.FeedUpdater.Domain.Models;

namespace PodcastManager.FeedUpdater.Domain.Repositories;

public interface IEpisodeRepository
{
    Task<(long, long)> Save(int code, Item[] feedItems);
    Task<long> EpisodeCount(int code);
}