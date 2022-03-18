using System.Threading.Tasks;
using PodcastManager.FeedUpdater.Domain.Adapters;

namespace PodcastManager.FeedUpdater.Doubles.Feed;

public class FeedDummy : IFeedAdapter
{
    public virtual Task<Domain.Models.Feed> Get(string feedUrl) =>
        Task.FromResult(Domain.Models.Feed.Empty());
}