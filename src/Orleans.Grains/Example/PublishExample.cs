using OrleansGrainInterfaces.Example;

namespace Orleans.Grains.Example;

public class PublishExample : Grain, IPublishExample
{
    public Task UpdateGreetingByStreamAsync(string newGreeting)
    {
        throw new NotImplementedException();
    }
}
