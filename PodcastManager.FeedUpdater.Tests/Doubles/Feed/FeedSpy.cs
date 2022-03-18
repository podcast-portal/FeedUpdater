using System.Threading.Tasks;

namespace PodcastManager.FeedUpdater.Doubles.Feed;

public class FeedSpy : FeedStub
{
    public SpyHelper<string> GetSpy { get; } = new();
    public override Task<Domain.Models.Feed> Get(string feedUrl)
    {
        GetSpy.Call(feedUrl);
        return base.Get(feedUrl);
    }
}