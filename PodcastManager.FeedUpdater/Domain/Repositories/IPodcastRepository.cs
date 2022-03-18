using PodcastManager.FeedUpdater.Domain.Models;
using PodcastManager.FeedUpdater.Messages;

namespace PodcastManager.FeedUpdater.Domain.Repositories;

public interface IPodcastRepository
{
    Task<IReadOnlyCollection<UpdatePodcast>> ListPodcastToUpdate();
    Task<IReadOnlyCollection<UpdatePublishedPodcast>> ListPublishedPodcastToUpdate();
    Task SaveFeedData(int code, Feed feed);
    Task UpdateStatus(int code, PodcastStatus status, string errorMessage = "");
}