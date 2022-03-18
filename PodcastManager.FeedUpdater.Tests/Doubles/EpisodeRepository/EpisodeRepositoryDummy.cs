using System.Threading.Tasks;
using PodcastManager.FeedUpdater.Domain.Models;
using PodcastManager.FeedUpdater.Domain.Repositories;

namespace PodcastManager.FeedUpdater.Doubles.EpisodeRepository;

public class EpisodeRepositoryDummy : IEpisodeRepository
{
    public virtual Task<(long, long)> Save(int code, Item[] feedItems) =>
        Task.FromResult((0L, 0L));

    public virtual Task<long> EpisodeCount(int code) => Task.FromResult(0L);
}