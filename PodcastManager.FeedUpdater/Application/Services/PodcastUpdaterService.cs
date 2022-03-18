using System.Diagnostics;
using PodcastManager.FeedUpdater.Adapters;
using PodcastManager.FeedUpdater.Domain.Adapters;
using PodcastManager.FeedUpdater.Domain.Exceptions;
using PodcastManager.FeedUpdater.Domain.Interactors;
using PodcastManager.FeedUpdater.Domain.Models;
using PodcastManager.FeedUpdater.Domain.Repositories;
using PodcastManager.FeedUpdater.Messages;
using Serilog;

namespace PodcastManager.FeedUpdater.Application.Services;

public class PodcastUpdaterService : IPodcastUpdaterInteractor
{
    private ILogger logger = null!;
    private IFeedAdapter feedAdapter = null!;
    private IPodcastRepository podcastRepository = null!;
    private IEpisodeRepository episodeRepository = null!;
    private IDateTimeAdapter dateTime = null!;

    public void SetLogger(ILogger logger) => this.logger = logger;
    public void SetFeed(IFeedAdapter feedAdapter) => this.feedAdapter = feedAdapter;
    public void SetDateTime(IDateTimeAdapter dateTime) => this.dateTime = dateTime;
    public void SetPodcastRepository(IPodcastRepository podcastRepository) =>
        this.podcastRepository = podcastRepository;
    public void SetEpisodeRepository(IEpisodeRepository episodeRepository) =>
        this.episodeRepository = episodeRepository;

    public async Task Execute(UpdatePodcast podcast)
    {
        var sw = Stopwatch.StartNew();
        
        var (code, title, feedUrl, isPublished, currentErrors) = podcast;

        var feedSw = Stopwatch.StartNew();
        long feedElapsed;
        Feed feed;
        Exception? error;
        try
        {
            (feed, error) = await TryGetFeedData();
        }
        finally
        {
            feedSw.Stop();
            feedElapsed = feedSw.ElapsedMilliseconds;
        }

        if (error != null)
        {
            var now = dateTime.Now();
            var status = new PodcastStatus(
                isPublished
                    ? GetNextPublishedSchedule(now, currentErrors)
                    : GetNextSchedule(now, currentErrors),
                LastTimeUpdated: now
            );
            await podcastRepository.UpdateStatus(code, status, error.Message);
            return;
        }

        long newEpisodes;
        long updatedEpisodes;

        var dbSw = Stopwatch.StartNew();
        long dbElapsed;
        try
        {
            (newEpisodes, updatedEpisodes) = await UpdateDatabaseWithoutErrors();
        }
        finally
        {
            dbSw.Stop();
            dbElapsed = dbSw.ElapsedMilliseconds;
        }

        sw.Stop();
        Action<string, object[]> log = newEpisodes + updatedEpisodes == 0 ? logger.Debug : logger.Information;
        log(
            "Podcast updated: {Podcast}, total episodes: {Total}, new: {New}, updated: {Updated} in {Time:N0}ms | {TimeFeed:}ms {TimeDB}ms",
            new object[]
            {
                title, feed.Items.Length, newEpisodes, updatedEpisodes, sw.ElapsedMilliseconds, feedElapsed, dbElapsed
            });

        async Task<(Feed, Exception?)> TryGetFeedData()
        {
            Exception? exception = null;
            var processedFeed = Feed.Empty();
            
            try
            {
                processedFeed = await feedAdapter.Get(feedUrl);
                return (processedFeed, exception);
            }
            catch (ServerErrorException e)
            {
                logger.Error("Server error ({ErrorCode} {ErrorReason}) for {Podcast}",
                    e.Code,
                    e.Reason,
                    podcast);
                exception = e;
            }
            catch (TryToParseErrorException e)
            {
                logger.Error("Parsing XML error for {Podcast}",
                    podcast);
                exception = e;
            }
            catch (Exception e)
            {
                logger.Error(e, "generic error with {Podcast}", podcast);
                exception = e;
            }

            return (processedFeed, exception);
        }

        async Task<(long, long)> UpdateDatabaseWithoutErrors()
        {
            await podcastRepository.SaveFeedData(code, feed);
            var (newCount, updateCount) = await episodeRepository.Save(code, feed.Items);
            var totalEpisodes = await episodeRepository.EpisodeCount(code);
            var now = dateTime.Now();
            var status = new PodcastStatus(
                isPublished ? GetNextPublishedSchedule(now) : GetNextSchedule(now),
                totalEpisodes,
                now
            );
            await podcastRepository.UpdateStatus(code, status);
            
            return (newCount, updateCount);
        }
    }

    private static DateTime GetNextSchedule(DateTime now, int errorCount = 0) =>
        now.Add(FeedUpdaterConfiguration.PodcastNextSchedule * (errorCount + 1));

    private static DateTime GetNextPublishedSchedule(DateTime now, int errorCount = 0) =>
        now.Add(FeedUpdaterConfiguration.PublishedPodcastNextSchedule * (errorCount + 1));
}