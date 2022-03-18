using System.Net;
using PodcastManager.FeedUpdater.Domain.Adapters;
using PodcastManager.FeedUpdater.Domain.Exceptions;
using PodcastManager.FeedUpdater.Domain.Interactors;
using PodcastManager.FeedUpdater.Domain.Models;

namespace PodcastManager.FeedUpdater.CrossCutting.Http;

public class FeedAdapter : IFeedAdapter
{
    private IHttpClientFactory httpClientFactory = null!;
    private IFeedConverterInteractor converter = null!;

    public void SetHttpClientFactory(IHttpClientFactory httpClientFactory) =>
        this.httpClientFactory = httpClientFactory;
    public void SetConverter(IFeedConverterInteractor converter) =>
        this.converter = converter;

    public async Task<Feed> Get(string feedUrl)
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync(feedUrl);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new ServerErrorException(response.StatusCode, response.ReasonPhrase);
        var body = await response.Content.ReadAsStringAsync();
        return converter.Execute(body);
    }
}