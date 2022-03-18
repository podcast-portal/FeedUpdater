using PodcastManager.FeedUpdater.CrossCutting.IoC;
using PodcastManager.FeedUpdater.CrossCutting.Mongo;
using PodcastManager.FeedUpdater.CrossCutting.Rabbit;
using RabbitMQ.Client;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ApplicationName", "Feed Updater")
    .MinimumLevel.Information()
    .CreateLogger();

Log.Logger.Information("Feed Updater service starting");

var closing = new AutoResetEvent(false);

MongoConfiguration.SetConventions();
var repositoryFactory = new RepositoryFactory();
repositoryFactory.SetLogger(Log.Logger);

var connectionFactory = new ConnectionFactory { Uri = new Uri(RabbitConfiguration.Host) };

var interactorFactory = new InteractorFactory();
interactorFactory.SetConnectionFactory(connectionFactory);
interactorFactory.SetRepositoryFactory(repositoryFactory);
interactorFactory.SetLogger(Log.Logger);

var listener = new RabbitUpdaterListenerAdapter();
listener.SetInteractorFactory(interactorFactory);
listener.SetConnectionFactory(connectionFactory);
listener.SetLogger(Log.Logger);
listener.Listen();


Console.CancelKeyPress += OnExit;
closing.WaitOne();
listener.Dispose();

void OnExit(object? sender, ConsoleCancelEventArgs args)
{
    Log.Logger.Information("Service Feed Updater is exiting");
    closing.Set();
}