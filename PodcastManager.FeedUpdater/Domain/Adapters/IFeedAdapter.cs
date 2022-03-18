using PodcastManager.FeedUpdater.Domain.Models;

namespace PodcastManager.FeedUpdater.Domain.Adapters;

public interface IFeedAdapter
{
    Task<Feed> Get(string feedUrl);
}