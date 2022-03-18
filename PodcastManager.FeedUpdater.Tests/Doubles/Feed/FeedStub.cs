using System;
using System.Threading.Tasks;
using PodcastManager.FeedUpdater.Domain.Models;

namespace PodcastManager.FeedUpdater.Doubles.Feed;

public class FeedStub : FeedDummy
{
    public override Task<Domain.Models.Feed> Get(string feedUrl)
    {
        return Task.FromResult(SingleFeed);
    }

    public Domain.Models.Feed SingleFeed { get; } =
        new Domain.Models.Feed("Podcast 1")
        {
            Items = new []
            {
                new Item(
                    "Ep 04",
                    new DateTime(2020, 4, 2),
                    "g04",
                    new Enclosure("file4", 4123, "type1")),
                new Item(
                    "Ep 03",
                    new DateTime(2020, 3, 2),
                    "g03",
                    new Enclosure("file3", 3123, "type1")),
                new Item(
                    "Ep 02",
                    new DateTime(2020, 2, 2),
                    "g02",
                    new Enclosure("file2", 2123, "type1")),
                new Item(
                    "Ep 01",
                    new DateTime(2020, 1, 2),
                    "g01",
                    new Enclosure("file1", 123, "type1")),
            }
        };
}