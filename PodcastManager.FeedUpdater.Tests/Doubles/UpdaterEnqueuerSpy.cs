using System;
using System.Collections.Generic;
using PodcastManager.FeedUpdater.Adapters;
using PodcastManager.FeedUpdater.Messages;

namespace PodcastManager.FeedUpdater.Doubles;

public class UpdaterEnqueuerSpy : IUpdaterEnqueuerAdapter
{
    public SpyHelper<UpdatePodcast> EnqueueUpdatePodcastSpy { get; } = new();
    public SpyHelper<IReadOnlyCollection<UpdatePodcast>> EnqueueUpdatePodcastsSpy { get; } = new();
    public SpyHelper<IReadOnlyCollection<UpdatePublishedPodcast>> EnqueueUpdatePublishedPodcastsSpy { get; } = new();

    public void EnqueueUpdateAllPodcasts()
    {
        throw new NotImplementedException();
    }

    public void EnqueueUpdateAllPublishedPodcasts()
    {
        throw new NotImplementedException();
    }

    public void EnqueueUpdatePodcast(UpdatePodcast podcast) =>
        EnqueueUpdatePodcastSpy.Call(podcast);

    public void EnqueueUpdatePodcasts(IReadOnlyCollection<UpdatePodcast> podcasts) =>
        EnqueueUpdatePodcastsSpy.Call(podcasts);

    public void EnqueueUpdatePublishedPodcasts(IReadOnlyCollection<UpdatePublishedPodcast> podcasts) =>
        EnqueueUpdatePublishedPodcastsSpy.Call(podcasts);
}