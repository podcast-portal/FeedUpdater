using PodcastManager.FeedUpdater.Domain.Interactors;

namespace PodcastManager.FeedUpdater.Doubles.FeedCleanerInteractor;

public class FeedConverterSpy : IFeedConverterInteractor
{
    public SpyHelper<string> ProcessSpy { get; } = new();

    public Domain.Models.Feed Execute(string data)
    {
        ProcessSpy.Call(data);
        return Domain.Models.Feed.Empty();
    }
}