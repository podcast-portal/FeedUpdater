using System.Net;
using System.Net.Http;
using RichardSzalay.MockHttp;

namespace PodcastManager.FeedUpdater.Doubles.HttpClientFactory;

public class HttpClientFactorySpy : HttpClientFactoryStub
{
    public SpyHelper<string> GetSpy { get; } = new();

    public override HttpClient CreateClient(string name)
    {
        var mock = new MockHttpMessageHandler();

        foreach (var url in Urls)
            mock.When(url)
                .Respond(HttpStatusCode.OK, new StringContent(Data[url]))
                .With(x =>
                {
                    GetSpy.Call(x.RequestUri?.ToString() ?? "");
                    return true;
                });
        
        return mock.ToHttpClient();
    }
}