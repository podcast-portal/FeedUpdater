using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PodcastManager.FeedUpdater.Domain.Adapters;
using PodcastManager.FeedUpdater.Doubles.FeedCleanerInteractor;
using PodcastManager.FeedUpdater.Doubles.HttpClientFactory;

namespace PodcastManager.FeedUpdater.CrossCutting.Http;

public class FeedAdapterTests
{
    private IFeedAdapter adapter = null!;
    private HttpClientFactorySpy clientFactorySpy = null!;
    private FeedConverterSpy converterSpy = null!;

    private void CreateAdapter()
    {
        clientFactorySpy = new HttpClientFactorySpy();
        converterSpy = new FeedConverterSpy();
        
        var newAdapter = new FeedAdapter();
        newAdapter.SetHttpClientFactory(clientFactorySpy);
        newAdapter.SetConverter(converterSpy);
        adapter = newAdapter;
    }

    [SetUp]
    public void SetUp() => CreateAdapter();

    [Test]
    public void Constructor_InheritsFromFeedAdapterInterface() => 
        adapter.Should().BeAssignableTo<IFeedAdapter>();

    [Test]
    public async Task Get_SendUrlToHttpClient()
    {
        const string url = "https://feed.podcast1.com/";
        await adapter.Get(url);
        
        clientFactorySpy.GetSpy.ShouldBeCalledOnce();
        clientFactorySpy.GetSpy.LastParameter.Should().Be(url);
    }

    [Test]
    public async Task Get_ShouldPassReturnOfClientToFeedProcess()
    {
        var feedUrl = HttpClientFactoryStub.Urls[0];
        await adapter.Get(feedUrl);

        converterSpy.ProcessSpy.ShouldBeCalledOnce();
        converterSpy.ProcessSpy.LastParameter.Should().Be(HttpClientFactoryStub.Data[feedUrl]);
    }
}