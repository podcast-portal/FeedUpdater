using System.Threading.Tasks;
using PodcastManager.FeedUpdater.Domain.Models;

namespace PodcastManager.FeedUpdater.Doubles.EpisodeRepository;

public class EpisodeRepositorySpy : EpisodeRepositoryStub
{
    public SpyHelper<(int, Item[])> SaveSpy { get; } = new();
    public SpyHelper<int> EpisodeCountSpy { get; } = new();

    public override Task<(long, long)> Save(int code, Item[] feedItems)
    {
        SaveSpy.Call((code, feedItems));
        return base.Save(code, feedItems);
    }

    public override Task<long> EpisodeCount(int code)
    {
        EpisodeCountSpy.Call(code);
        return base.EpisodeCount(code);
    }
}