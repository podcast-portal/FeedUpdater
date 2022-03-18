using MongoDB.Driver;
using PodcastManager.FeedUpdater.Domain.Factories;
using PodcastManager.FeedUpdater.Domain.Repositories;
using PodcastManager.FeedUpdater.CrossCutting.Mongo;
using PodcastManager.FeedUpdater.CrossCutting.System;
using Serilog;

namespace PodcastManager.FeedUpdater.CrossCutting.IoC;

public class RepositoryFactory : IRepositoryFactory
{
    private ILogger logger = null!;

    public void SetLogger(ILogger logger) => this.logger = logger;

    public IEpisodeRepository CreateEpisode()
    {
        var repository = new MongoEpisodeRepository();
        repository.SetDatabase(GetDatabase());
        return repository;
    }

    public IPodcastRepository CreatePodcast()
    {
        var repository = new MongoPodcastRepository();
        repository.SetDatabase(GetDatabase());
        repository.SetDateTime(new UtcDateTimeAdapter());
        repository.SetLogger(logger);
        return repository;
    }

    private static IMongoDatabase GetDatabase()
    {
        var client = new MongoClient(MongoConfiguration.MongoUrl);
        var database = client.GetDatabase(MongoConfiguration.MongoDatabase);
        return database;
    }
}