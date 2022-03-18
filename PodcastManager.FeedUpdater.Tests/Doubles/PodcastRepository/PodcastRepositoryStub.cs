using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PodcastManager.FeedUpdater.Messages;

namespace PodcastManager.FeedUpdater.Doubles.PodcastRepository;

public class PodcastRepositoryStub : PodcastRepositoryDummy
{
    public readonly UpdatePodcast[] Podcasts = {
        new UpdatePublishedPodcast(1, "Podcast 1", "https://feedpodcast1.com/rss"),
        new UpdatePublishedPodcast(2, "Podcast 2", "https://feedpodcast2.com/rss"),
        new UpdatePublishedPodcast(3, "Podcast 3", "https://feedpodcast3.com/rss"),
        new(4, "Podcast 4", "https://feedpodcast4.com/rss"),
        new(5, "Podcast 5", "https://feedpodcast5.com/rss")
    };
    
    public override Task<IReadOnlyCollection<UpdatePodcast>> ListPodcastToUpdate()
    {
        return Task.FromResult(Podcasts as IReadOnlyCollection<UpdatePodcast>);
    }

    public override Task<IReadOnlyCollection<UpdatePublishedPodcast>> ListPublishedPodcastToUpdate()
    {
        return Task.FromResult(Podcasts
            .Cast<UpdatePublishedPodcast>()
            .Take(3).ToList() as IReadOnlyCollection<UpdatePublishedPodcast>);
    }
}