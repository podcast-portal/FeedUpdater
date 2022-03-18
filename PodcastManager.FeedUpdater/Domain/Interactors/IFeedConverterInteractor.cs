using PodcastManager.FeedUpdater.Domain.Models;

namespace PodcastManager.FeedUpdater.Domain.Interactors;

public interface IFeedConverterInteractor
{
    Feed Execute(string data);
}