namespace PodcastManager.FeedUpdater.Domain.Exceptions;

public class PropertyNotExistsException : Exception
{
    public string Property { get; }
    public string Parent { get; }

    public PropertyNotExistsException(string property, string parent)
    {
        Property = property;
        Parent = parent;
    }
}