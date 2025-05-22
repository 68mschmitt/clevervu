namespace OrleansGrainInterfaces.Example;

[Alias("OrleansGrainInterfaces.Example.IPublishExample")]
public interface IPublishExample : IGrainWithStringKey
{
    [Alias("UpdateGreetingByStreamAsync")]
    Task UpdateGreetingByStreamAsync(string newGreeting);
}
