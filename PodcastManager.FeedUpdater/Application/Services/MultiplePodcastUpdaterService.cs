using PodcastManager.FeedUpdater.Adapters;
using PodcastManager.FeedUpdater.Domain.Interactors;
using PodcastManager.FeedUpdater.Domain.Repositories;
using PodcastManager.FeedUpdater.Messages;
using Serilog;

namespace PodcastManager.FeedUpdater.Application.Services;

public class MultiplePodcastUpdaterService : IMultiplePodcastUpdaterInteractor
{
    private IPodcastRepository repository = null!;
    private IUpdaterEnqueuerAdapter enqueuer = null!;
    private ILogger logger = null!;

    public void SetRepository(IPodcastRepository repository) => this.repository = repository;
    public void SetEnqueuer(IUpdaterEnqueuerAdapter enqueuer) => this.enqueuer = enqueuer;
    public void SetLogger(ILogger logger) => this.logger = logger;

    public async Task Execute()
    {
        if (!enqueuer.CanEnqueue()) return;
        var podcasts = await repository.ListPodcastToUpdate();
        EnqueuePodcasts(podcasts);
        logger.Information("Total podcasts enqueued {Total}", podcasts.Count);
    }

    public async Task ExecutePublished()
    {
        var podcasts = await repository.ListPublishedPodcastToUpdate();
        EnqueuePublishedPodcasts(podcasts);
        logger.Information("Total published podcasts enqueued {Total}", podcasts.Count);
    }

    private void EnqueuePodcasts(IReadOnlyCollection<UpdatePodcast> podcasts) => 
        enqueuer.EnqueueUpdatePodcasts(podcasts);

    private void EnqueuePublishedPodcasts(IReadOnlyCollection<UpdatePublishedPodcast> podcasts) => 
        enqueuer.EnqueueUpdatePublishedPodcasts(podcasts);
}