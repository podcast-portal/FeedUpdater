using MongoDB.Bson.Serialization.Conventions;

namespace PodcastManager.FeedUpdater.CrossCutting.Mongo;

public static class MongoConfiguration
{
    public static readonly string MongoUrl =
        Environment.GetEnvironmentVariable("MongoUrl")
        ?? "mongodb://127.0.0.1:27017/";
    public static readonly string MongoDatabase =
        Environment.GetEnvironmentVariable("MongoDatabase")
        ?? "podcastManager";

    public static void SetConventions()
    {
        ConventionRegistry.Register(
            "camelCase",
            new ConventionPack{new CamelCaseElementNameConvention()},
            _ => true);
        ConventionRegistry.Register(
            "ignoreNull",
            new ConventionPack{new IgnoreIfNullConvention(true)},
            _ => true);
        ConventionRegistry.Register(
            "ignoreExtraElements",
            new ConventionPack{new IgnoreExtraElementsConvention(true)},
            _ => true);
    }  
}