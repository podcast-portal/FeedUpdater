using System.Text;
using Newtonsoft.Json;
using PodcastManager.FeedUpdater.Adapters;
using PodcastManager.FeedUpdater.Domain.Factories;
using PodcastManager.FeedUpdater.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace PodcastManager.FeedUpdater.CrossCutting.Rabbit;

public class RabbitUpdaterListenerAdapter : IListenerAdapter, IDisposable
{
    private IConnection connection = null!;
    private IModel channel = null!;
    private ILogger logger = null!;
    private IInteractorFactory interactorFactory = null!;

    public void Listen()
    {
        ListenTo<UpdatePodcast>(RabbitConfiguration.UpdatePodcastQueue,
            podcast => interactorFactory.CreatePodcastUpdater().Execute(podcast), 20, false);
        ListenTo<UpdatePublishedPodcast>(RabbitConfiguration.UpdatePublishedPodcastQueue,
            podcast => interactorFactory.CreatePodcastUpdater().Execute(podcast));
        ListenTo(RabbitConfiguration.UpdatePodcastsQueue,
            () => interactorFactory.CreateMultiple().Execute());
        ListenTo(RabbitConfiguration.UpdatePublishedPodcastsQueue,
            () => interactorFactory.CreateMultiple().ExecutePublished());
    }

    public void SetConnectionFactory(IConnectionFactory connectionFactory)
    {
        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();
    }
    public void SetLogger(ILogger logger) => this.logger = logger;
    public void SetInteractorFactory(IInteractorFactory interactorFactory) =>
        this.interactorFactory = interactorFactory;
    
    public void Dispose()
    {
        connection.Dispose();
        channel.Dispose();
        GC.SuppressFinalize(this);
    }

    private void ListenTo<T>(string queue, Func<T, Task> action, ushort prefetch = 20, bool isGlobal = true)
    {
        logger.Information("listening to: {Queue}", queue);
        ConfigureChannel();
        channel.BasicConsume(queue, false, ConfigureConsumer());

        async Task TryProcessMessage(BasicDeliverEventArgs basicDeliverEventArgs)
        {
            try
            {
                await ProcessMessage(basicDeliverEventArgs);
                channel.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error: '{Error}' processing message {Queue}", e.Message, queue);
                channel.BasicNack(basicDeliverEventArgs.DeliveryTag, false, false);
            }
        }
        async Task ProcessMessage(BasicDeliverEventArgs args)
        {
            var json = Encoding.UTF8.GetString(args.Body.ToArray());
            if (string.IsNullOrEmpty(json)) json = "{}";
            
            var message = JsonConvert.DeserializeObject<T>(json);
            
            logger.Debug("Message Received: {Message}", message);
            await action(message!);
        }
        void ConfigureChannel()
        {
            channel.QueueDeclare(queue, true, false, false);
            channel.BasicQos(0, prefetch, isGlobal);
        }
        EventingBasicConsumer ConfigureConsumer()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, args) => await TryProcessMessage(args);
            return consumer;
        }
    }
    private void ListenTo(string queue, Func<Task> action, ushort prefetch = 20, bool isGlobal = true)
    {
        logger.Information("listening to: {Queue}", queue);
        ConfigureChannel();
        channel.BasicConsume(queue, false, ConfigureConsumer());

        async Task TryProcessMessage(BasicDeliverEventArgs basicDeliverEventArgs)
        {
            try
            {
                await ProcessMessage();
                channel.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error: '{Error}' processing message {Queue}", e.Message, queue);
                channel.BasicNack(basicDeliverEventArgs.DeliveryTag, false, false);
            }
        }
        async Task ProcessMessage()
        {
            logger.Debug("Message Received on queue: {Queue}", queue);
            await action();
        }
        void ConfigureChannel()
        {
            channel.QueueDeclare(queue, true, false, false);
            channel.BasicQos(0, prefetch, isGlobal);
        }
        EventingBasicConsumer ConfigureConsumer()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (_, args) => await TryProcessMessage(args);
            return consumer;
        }
    }

}