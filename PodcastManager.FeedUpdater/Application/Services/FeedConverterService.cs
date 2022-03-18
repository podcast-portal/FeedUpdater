using System.Xml.Linq;
using PodcastManager.FeedUpdater.Domain.Exceptions;
using PodcastManager.FeedUpdater.Domain.Interactors;
using PodcastManager.FeedUpdater.Domain.Models;

namespace PodcastManager.FeedUpdater.Application.Services;

public class FeedConverterService : IFeedConverterInteractor
{
    public Feed Execute(string data)
    {
        if (string.IsNullOrWhiteSpace(data)) return Feed.Empty();

        if (!TryParseRss(data, out var rss))
            throw new TryToParseErrorException();
        // var rss = XElement.Parse(data);
        
        var channel = rss.Element("channel");

        if (channel == null) throw new ChannelNotFoundException();


        var feed = new Feed(
                GetElementValue(channel, "title"),
                GetElementNullableValue(channel, "link"),
                GetElementNullableValue(channel, "description"),
                GetElementNullableValue(channel, "language"),
                GetElementNullableValue(channel, "itunes:image", ConvertToImage),
                GetElementNullableValue(channel, "itunes:subtitle"),
                GetElementNullableValue(channel, "itunes:summary"),
                GetElementNullableValue(channel, "itunes:owner", ConvertToOwner)
        )
        {
            Categories = GetElementValues(channel, "itunes:category", ConvertToCategory),
            Items = GetElementValues(channel, "item", ConvertToItem)
                .Where(x => !x.IsEmpty)
                .ToArray()
        };
        
        return feed;
    }

    private static bool TryParseRss(string data, out XElement rss)
    {
        try
        {
            rss = XElement.Parse(data);
            return true;
        }
        catch (Exception)
        {
            rss = null!;
            return false;
        }
    }

    private Item ConvertToItem(XElement element)
    {
        if (!IsValidItem())
            return Item.Empty();
        
        return new Item(
            GetElementValue(element, "title"),
            GetElementValue(element, "pubDate", x => ConvertToDate(x.Value)),
            GetElementValue(element, "guid"),
            GetElementValue(element, "enclosure", ConvertToEnclosure),
            GetElementNullableValue(element, "link"),
            GetElementNullableValue(element, "description"),
            GetElementNullableValue(element, "itunes:subtitle"),
            GetElementNullableValue(element, "itunes:summary"),
            GetElementNullableValue(element, "itunes:author"),
            GetElementNullableValue(element, "itunes:duration", ConvertToTimeSpan), GetElementNullableValue(element, "itunes:image", ConvertToImage));

        bool IsValidItem() =>
            !(element.Element("enclosure") == null ||
              element.Element("guid") == null ||
              element.Element("pubDate") == null ||
              string.IsNullOrWhiteSpace(element.Element("pubDate")?.Value));
    }

    private TimeSpan ConvertToTimeSpan(XElement element)
    {
        if (TryConvertToTimeSpan(element, out var value, out var exception))
            return value!.Value;

        throw exception!;
    }

    private bool TryConvertToTimeSpan(XElement element, out TimeSpan? value, out Exception? exception)
    {
        try
        {
            var cleaned = CleanTimeSpan(element.Value);
            var numbers = cleaned
                .Split(':')
                .Select(int.Parse)
                .ToArray();

            value = numbers.Length switch
            {
                >= 3 => new TimeSpan(numbers[^3], numbers[^2], numbers[^1]),
                2 => new TimeSpan(0, numbers[0], numbers[1]),
                _ => new TimeSpan(0, 0, numbers[0]),
            };
            exception = null;
            return true;
        }
        catch (Exception e)
        {
            value = null;
            exception = e;
            return false;
        }
    }

    private static string CleanTimeSpan(string value) =>
        value
            .Replace(" min", ":00");

    private Enclosure ConvertToEnclosure(XElement element)
    {
        var lengthText = GetAttributeNullableValue(element, "length");
        
        if (!TryParseLength(lengthText, out var length))
            throw new NumberBadFormattedException(lengthText);
        
        return new Enclosure(
            GetAttributeValue(element, "url"),
            length,
            GetAttributeValue(element, "type"));
    }

    public static bool TryParseLength(string? text, out long length)
    {
        length = 0;
        if (string.IsNullOrWhiteSpace(text)) return true;
        if (text.Contains(':')) return true;

        if (int.TryParse(text, out var valueInt))
        {
            length = valueInt;
            return true;
        }

        if (double.TryParse(text, out var valueDouble))
        {
            length = Convert.ToInt64(valueDouble * 1024 * 1024);
            return true;
        }

        return false;
    }

    public static DateTime ConvertToDate(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text));

        if (text.Length < 16)
        {
            if (DateTime.TryParse(text, out var firstDate))
                return firstDate;
            throw new DateTimeBadFormattedException(text);
        }
        
        var cleanedText = Clean(text);

        if (!DateTime.TryParse(cleanedText, out var date))
            throw new DateTimeBadFormattedException(cleanedText);
        
        return date.ToUniversalTime();

    }

    private static string Clean(string text)
    {
        var fixedText = text
            .Replace("CEST", "+0200")
            .Replace("CET", "+0100")
            .Replace("PST", "-0800")
            .Replace("PDT", "-0700")
            .Replace("EST", "-0500")
            .Replace("EDT", "-0400")
            .Replace("GMT", "+0000")
            .Replace(" 3 ", " 03 ")
            .Replace(" 9 ", " 09 ");
        return FixWeekDay(fixedText);
    }

    private static string FixWeekDay(string text)
    {
        var dateString = string.Join(" ", text.Split(' ')[1..4]);
        var date = DateOnly.Parse(dateString);
        return date.DayOfWeek.ToString()[..3] + text[4..];
    }

    private static string ConvertToCategory(XElement element) =>
        GetAttributeValue(element, "text");

    private static string GetAttributeValue(XElement element, string attributeName)
    {
        var attribute = element.Attribute(attributeName);
        if (attribute == null) throw new HrefAttributeNotFoundInImageException();
        return attribute.Value;
    }

    private static string? GetAttributeNullableValue(XElement element, string attributeName)
    {
        var attribute = element.Attribute(attributeName);
        return attribute?.Value;
    }

    private static Image ConvertToImage(XElement element) =>
        new(GetAttributeValue(element, "href"));

    private static Owner ConvertToOwner(XElement element)
    {
        return new Owner(
            GetElementNullableValue(element, "itunes:name"),
            GetElementNullableValue(element, "itunes:email")
        );
    }

    private static string? GetElementNullableValue(XContainer channel, string elementName) => 
        channel.Element(GetXName(elementName))?.Value;
    private static string[] GetElementNullableValues(XContainer channel, string elementName) =>
        channel.Elements(GetXName(elementName))
            .Select(x => x.Value)
            .ToArray();

    private static T? GetElementNullableValue<T>(XContainer channel, string elementName, Func<XElement, T> converter)
    {
        var element = channel.Element(GetXName(elementName));
        return element == null ? default : converter(element);
    }
    private static string GetElementValue(XElement channel, string elementName)
    {
        var element = channel.Element(GetXName(elementName));
        if (element == null)
            throw new PropertyNotExistsException(elementName, channel.Name.ToString());
        return element.Value;
    }
    private static string[] GetElementValues(XElement channel, string elementName)
    {
        var elements = channel.Elements(GetXName(elementName));
        if (elements == null)
            throw new PropertyNotExistsException(elementName, channel.Name.ToString());
        return elements
            .Select(x => x.Value)
            .ToArray();
    }

    private static T[] GetElementValues<T>(XElement channel, string elementName, Func<XElement, T> converter)
    {
        var elements = channel.Elements(GetXName(elementName));
        if (elements == null)
            throw new PropertyNotExistsException(elementName, channel.Name.ToString());
        return elements
            .Select(converter)
            .ToArray();
    }

    private static T GetElementValue<T>(XElement channel, string elementName, Func<XElement, T> converter)
    {
        var element = channel.Element(GetXName(elementName));
        if (element == null) 
            throw new PropertyNotExistsException(elementName, channel.Name.ToString());
        return converter(element);
    }

    private static XName GetXName(string elementName)
    {
        if (!elementName.Contains(':'))
            return elementName;

        var items = elementName.Split(':');
        var ns = Namespaces[items[0]];
        return XName.Get(items[1], ns);
    }

    private static readonly Dictionary<string, string> Namespaces = new()
    {
        {"itunes", "http://www.itunes.com/dtds/podcast-1.0.dtd"}
    };
}