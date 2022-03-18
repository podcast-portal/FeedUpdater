using System.Linq.Expressions;
using MongoDB.Driver;

namespace PodcastManager.FeedUpdater.CrossCutting.Mongo;

public abstract class MongoRepository
{
    private IMongoDatabase database = null!;

    public void SetDatabase(IMongoDatabase database) =>
        this.database = database;

    protected IMongoCollection<T> GetCollection<T>(string name) =>
        database.GetCollection<T>(name);
}

public static class UpdateDefinitionExtension
{
    public static UpdateDefinition<T> SetOrUnset<T, TField>(this UpdateDefinitionBuilder<T> @this,
        Expression<Func<T, object>> field, TField value) =>
        value == null 
            ? @this.Unset(field)
            : @this.Set(field, value);
    public static UpdateDefinition<T> SetOrUnset<T, TField>(this UpdateDefinition<T> @this,
        Expression<Func<T, object>> field, TField value) =>
        value == null 
            ? @this.Unset(field)
            : @this.Set(field, value);
}
