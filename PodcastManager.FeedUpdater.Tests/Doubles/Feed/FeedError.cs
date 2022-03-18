using System.Net;
using System.Threading.Tasks;
using PodcastManager.FeedUpdater.Domain.Adapters;
using PodcastManager.FeedUpdater.Domain.Exceptions;

namespace PodcastManager.FeedUpdater.Doubles.Feed;

public class FeedError : IFeedAdapter
{
    public Task<Domain.Models.Feed> Get(string feedUrl)
    {
        throw new ServerErrorException(HttpStatusCode.NotFound, "Not found");
    }
}