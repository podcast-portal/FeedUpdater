using MongoDB.Driver;
using PodcastManager.FeedUpdater.Domain.Models;
using PodcastManager.FeedUpdater.Domain.Repositories;

namespace PodcastManager.FeedUpdater.CrossCutting.Mongo;

public class MongoEpisodeRepository : MongoRepository, IEpisodeRepository
{
    private readonly FilterDefinitionBuilder<Episode> filterBuilder;
    private readonly UpdateDefinitionBuilder<Episode> updateBuilder;

    public MongoEpisodeRepository()
    {
        filterBuilder = Builders<Episode>.Filter;
        updateBuilder = Builders<Episode>.Update;
    }

    public async Task<(long, long)> Save(int code, Item[] feedItems)
    {
        var collection = GetCollection<Episode>("episodes");
        var requests = new List<WriteModel<Episode>>();
        
        foreach (var item in feedItems)
        {
            var filter = filterBuilder
                .And(filterBuilder.Eq(x => x.PodcastCode, code),
                    filterBuilder.Eq(x => x.Guid, item.Guid));
            var update = updateBuilder
                .Set(x => x.PodcastCode, code)
                .Set(x => x.Title, item.Title)
                .Set(x => x.PublicationDate, item.PublicationDate)
                .Set(x => x.Guid, item.Guid)
                .Set(x => x.Enclosure, new Enclosure(
                    item.Enclosure.Url,
                    item.Enclosure.Length,
                    item.Enclosure.Type))
                .SetOrUnset(x => x.Link!, item.Link)
                .SetOrUnset(x => x.Description!, item.Description)
                .SetOrUnset(x => x.Subtitle!, item.Subtitle)
                .SetOrUnset(x => x.Summary!, item.Summary)
                .SetOrUnset(x => x.Author!, item.Author)
                .SetOrUnset(x => x.Duration!, item.Duration)
                .SetOrUnset(x => x.Image!, item.Image == null ? null : new Image(item.Image!.Href));
            // var episode = new Episode(code, item.Title, item.PublicationDate, item.Guid,
            //     item.Enclosure, item.Link, item.Description, item.Subtitle, item.Summary, item.Author,
            //     item.Duration, item.Image == null ? null : new Image(item.Image!.Href));

            // requests.Add(new ReplaceOneModel<Episode>(filter, episode) {IsUpsert = true});
            requests.Add(new UpdateOneModel<Episode>(filter, update) {IsUpsert = true});
        }

        if (requests.Count == 0) return (0, 0);

        var result = await collection.BulkWriteAsync(requests,
            new BulkWriteOptions {BypassDocumentValidation = true, IsOrdered = false});
        return (result.Upserts.Count, result.ModifiedCount);
    }

    public async Task<long> EpisodeCount(int code)
    {
        var collection = GetCollection<Episode>("episodes");
        var filter = filterBuilder.Eq(x => x.PodcastCode, code);
        return await collection.CountDocumentsAsync(filter);
    }
}