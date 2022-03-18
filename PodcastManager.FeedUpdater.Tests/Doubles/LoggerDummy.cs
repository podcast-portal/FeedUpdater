using Serilog;
using Serilog.Events;

namespace PodcastManager.FeedUpdater.Doubles;

public class LoggerDummy : ILogger
{
    public void Write(LogEvent logEvent)
    {
    }
}