using System.Threading.Tasks;
using PodcastManager.FeedUpdater.Domain.Models;

namespace PodcastManager.FeedUpdater.Doubles.EpisodeRepository;

public class EpisodeRepositoryStub : EpisodeRepositoryDummy
{
    public override Task<(long, long)> Save(int code, Item[] feedItems) => Task.FromResult((1L, 1L));
    public override Task<long> EpisodeCount(int code) => Task.FromResult(15L);
}