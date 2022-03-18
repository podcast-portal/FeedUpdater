using System.Collections.Generic;
using FluentAssertions;

namespace PodcastManager.FeedUpdater.Doubles;

public class SpyHelper
{
    private int timesCalled;
    
    public void Call() => timesCalled++;
    public void ShouldBeCalledOnce() => timesCalled.Should().Be(1);
    public void ShouldNeverBeCalled() => timesCalled.Should().Be(0);
    public void ShouldBeCalled(int times) => timesCalled.Should().Be(times);
}

public class SpyHelper<T> : SpyHelper
{
    private readonly IList<T> parameters = new List<T>();

    public IEnumerable<T> Parameters => parameters;
    public T LastParameter => parameters[^1];

    public void Call(T parameter)
    {
        parameters.Add(parameter);
        base.Call();
    }
}
