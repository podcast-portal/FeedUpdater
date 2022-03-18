using MongoDB.Driver;
using PodcastManager.FeedUpdater.Adapters;
using PodcastManager.FeedUpdater.Domain.Models;
using PodcastManager.FeedUpdater.Domain.Repositories;
using PodcastManager.FeedUpdater.Messages;
using Serilog;

namespace PodcastManager.FeedUpdater.CrossCutting.Mongo;

public class MongoPodcastRepository : MongoRepository, IPodcastRepository
{
    private IDateTimeAdapter dateTime = null!;
    private ILogger logger = null!;
    
    private readonly ExpressionFilterDefinition<FullPodcast> isPublished = new(x => x.IsPublished);

    public void SetDateTime(IDateTimeAdapter dateTime) => this.dateTime = dateTime;
    public void SetLogger(ILogger logger) => this.logger = logger;

    public Task<IReadOnlyCollection<UpdatePodcast>> ListPodcastToUpdate() =>
        ListPodcastsToUpdate(GetNeedsUpdate(dateTime.Now()));

    public async Task<IReadOnlyCollection<UpdatePublishedPodcast>> ListPublishedPodcastToUpdate()
    {
        var podcasts = await ListPodcastsToUpdate(isPublished, GetNeedsUpdate(dateTime.Now()));
        return podcasts
            .Select(x => new UpdatePublishedPodcast(x.Code, x.Title, x.Feed, x.CurrentErrors))
            .ToList();
    }

    public async Task SaveFeedData(int code, Feed feed)
    {
        var collection = GetCollection<FullPodcast>("podcasts");
        var filter = Builders<FullPodcast>.Filter.Eq(x => x.Code, code);
        var update = Builders<FullPodcast>.Update
                .Set(x => x.Imported.Feed!.Title, feed.Title)
                .SetOrUnset(x => x.Imported.Feed!.Description!, feed.Description)
                .SetOrUnset(x => x.Imported.Feed!.Language!, feed.Language)
                .SetOrUnset(x => x.Imported.Feed!.Link!, feed.Link)
                .SetOrUnset(x => x.Imported.Feed!.Subtitle!, feed.Subtitle)
                .SetOrUnset(x => x.Imported.Feed!.Summary!, feed.Summary)
                .SetOrUnset(x => x.Imported.Feed!.Image!,
                    feed.Image == null ? null : new Image(feed.Image.Href))
                .SetOrUnset(x => x.Imported.Feed!.Owner!, 
                    feed.Owner == null ? null : new Owner(feed.Owner.Name, feed.Owner.Email));
        await collection.UpdateOneAsync(filter, update);
    }

    public async Task UpdateStatus(int code, PodcastStatus status, string errorMessage = "")
    {
        var collection = GetCollection<FullPodcast>("podcasts");
        var filter = Builders<FullPodcast>.Filter.Eq(x => x.Code, code);
        var update = Builders<FullPodcast>.Update
            .Set(x => x.Status!.LastTimeUpdated, status.LastTimeUpdated)
            .Set(x => x.Status!.NextUpdate, status.NextUpdate);

        if (string.IsNullOrWhiteSpace(errorMessage))
            update = update
                .Set(x => x.Status!.ErrorCount, 0)
                .Unset(x => x.Status!.Errors);
        else
            update = update
                .Inc(x => x.Status!.ErrorCount, 1)
                .AddToSet(x => x.Status!.Errors, errorMessage);

        await collection.UpdateOneAsync(filter, update);
    }

    private async Task<IReadOnlyCollection<UpdatePodcast>> ListPodcastsToUpdate(
        params FilterDefinition<FullPodcast>[] filters)
    {
        var collection = GetCollection<FullPodcast>("podcasts");
        var cursor = await collection
            .Find(Builders<FullPodcast>.Filter.And(filters), new FindOptions {BatchSize = 10000})
            .Project(x => new UpdatePodcast(x.Code, x.Title, x.Feed, x.IsPublished, x.Status!.ErrorCount))
            .ToCursorAsync();

        var result = new List<UpdatePodcast>();
        var page = 1;

        while (await cursor.MoveNextAsync())
        {
            result.AddRange(cursor.Current);
            logger.Debug("Required page {Page}", page);
            page++;
        }

        return result;
    }

    private static ExpressionFilterDefinition<FullPodcast> GetNeedsUpdate(DateTime now) =>
        new(x => x.Status == null || x.Status.NextUpdate <= now );
}