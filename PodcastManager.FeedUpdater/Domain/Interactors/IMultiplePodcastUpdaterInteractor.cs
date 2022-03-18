namespace PodcastManager.FeedUpdater.Domain.Interactors;

public interface IMultiplePodcastUpdaterInteractor
{
    Task Execute();
    Task ExecutePublished();
}