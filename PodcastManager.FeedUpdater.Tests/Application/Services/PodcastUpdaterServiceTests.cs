using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PodcastManager.FeedUpdater.Adapters;
using PodcastManager.FeedUpdater.Domain.Adapters;
using PodcastManager.FeedUpdater.Domain.Interactors;
using PodcastManager.FeedUpdater.Domain.Models;
using PodcastManager.FeedUpdater.Domain.Repositories;
using PodcastManager.FeedUpdater.Doubles;
using PodcastManager.FeedUpdater.Doubles.EpisodeRepository;
using PodcastManager.FeedUpdater.Doubles.Feed;
using PodcastManager.FeedUpdater.Doubles.PodcastRepository;
using PodcastManager.FeedUpdater.Messages;
using Serilog;

namespace PodcastManager.FeedUpdater.Application.Services;

public class PodcastUpdaterServiceTests
{
    private IPodcastUpdaterInteractor service = null!;
    UpdatePodcast publishedPodcast = new UpdatePodcast(1, "Podcast 1", "https://feed.podcast1.com", true, 3);
    UpdatePodcast podcast = new UpdatePodcast(1, "Podcast 1", "https://feed.podcast1.com", false, 3);


    private void CreateService(
        IFeedAdapter? feed = null,
        IPodcastRepository? podcastRepository = null,
        IEpisodeRepository? episodeRepository = null,
        IDateTimeAdapter? dateTime = null,
        ILogger? logger = null)
    {
        var newService = new PodcastUpdaterService();
        newService.SetFeed(feed ?? new FeedDummy());
        newService.SetLogger(logger ?? new LoggerDummy());
        newService.SetDateTime(dateTime ?? new DateTimeStub());
        newService.SetPodcastRepository(podcastRepository ?? new PodcastRepositoryDummy());
        newService.SetEpisodeRepository(episodeRepository ?? new EpisodeRepositoryDummy());
        service = newService;
    }

    public class WithDummy : PodcastUpdaterServiceTests
    {
        [SetUp]
        public void SetUp()
        {
            CreateService();
        }
        
        [Test]
        public void Constructor_InheritsFromPodcastUpdaterInteractor() =>
            service.Should().BeAssignableTo<IPodcastUpdaterInteractor>();
    }

    public class WithSpy : PodcastUpdaterServiceTests
    {
        private FeedSpy feedSpy = null!;
        private PodcastRepositorySpy podcastRepositorySpy = null!;
        private EpisodeRepositorySpy episodeRepositorySpy = null!;
    
        [SetUp]
        public void SetUp()
        {
            feedSpy = new FeedSpy();
            podcastRepositorySpy = new PodcastRepositorySpy();
            episodeRepositorySpy = new EpisodeRepositorySpy();
        
            CreateService(feedSpy, podcastRepositorySpy, episodeRepositorySpy);
        }

        [Test]
        public async Task Execute_CallGetAtFeedAdapterOnce()
        {
            await service.Execute(podcast);
        
            feedSpy.GetSpy.ShouldBeCalledOnce();
            feedSpy.GetSpy.LastParameter.Should().Be(publishedPodcast.Feed);
        }

        [Test]
        public async Task Execute_WithoutErrorCallPodcastSaveImportedFromFeedAndSaveEpisodesOnce()
        {
            await service.Execute(podcast);

            podcastRepositorySpy.SaveFeedDataSpy.ShouldBeCalledOnce();
            podcastRepositorySpy.SaveFeedDataSpy.LastParameter
                .Should().BeEquivalentTo((podcast.Code, feedSpy.SingleFeed));

            episodeRepositorySpy.SaveSpy.ShouldBeCalledOnce();
            episodeRepositorySpy.SaveSpy.LastParameter
                .Should().BeEquivalentTo((podcast.Code, feedSpy.SingleFeed.Items));
        }

        [Test]
        public async Task Execute_ShouldCallUpdateStatusOnce()
        {
            await service.Execute(publishedPodcast);

            episodeRepositorySpy.EpisodeCountSpy.ShouldBeCalledOnce();
            episodeRepositorySpy.EpisodeCountSpy.LastParameter
                .Should().Be(publishedPodcast.Code);
            podcastRepositorySpy.UpdateStatusSpy.ShouldBeCalledOnce();
            podcastRepositorySpy.UpdateStatusSpy.LastParameter
                .Should().BeEquivalentTo((
                    publishedPodcast.Code,
                    new PodcastStatus(
                        new DateTime(2020, 6, 25, 10, 30, 0),
                        15,
                        new DateTime(2020, 6, 25, 10, 0, 0))
                )); 
        }
    }

    public class WithError : PodcastUpdaterServiceTests
    {
        private FeedError feedError = null!;
        private PodcastRepositorySpy podcastRepositorySpy = null!;
        private EpisodeRepositorySpy episodeRepositorySpy = null!;
    
        [SetUp]
        public void SetUp()
        {
            feedError = new FeedError();
            podcastRepositorySpy = new PodcastRepositorySpy();
            episodeRepositorySpy = new EpisodeRepositorySpy();
        
            CreateService(feedError, podcastRepositorySpy, episodeRepositorySpy);
        }

        [Test]
        public async Task Execute_WithErrorShouldNotCallEitherSaveImportedFromFeedAndSaveEpisodes()
        {
            await service.Execute(podcast);
            
            podcastRepositorySpy.SaveFeedDataSpy.ShouldNeverBeCalled();
            episodeRepositorySpy.SaveSpy.ShouldNeverBeCalled();
        }
        
        
        [Test]
        public async Task Execute_ShouldCallUpdateStatusOnce()
        {
            await service.Execute(publishedPodcast);

            podcastRepositorySpy.UpdateStatusSpy.ShouldBeCalledOnce();
            podcastRepositorySpy.UpdateStatusSpy.LastParameter
                .Should().BeEquivalentTo((
                    publishedPodcast.Code,
                    new PodcastStatus(
                        new DateTime(2020, 6, 25, 12, 0, 0),
                        LastTimeUpdated: new DateTime(2020, 6, 25, 10, 0, 0)),
                    "Not found"
                )); 
        }
    }
}