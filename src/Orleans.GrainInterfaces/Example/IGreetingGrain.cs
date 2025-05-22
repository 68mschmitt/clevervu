namespace OrleansGrainInterfaces.Example;

[Alias("OrleansGrainInterfaces.Example.IGreetingGrain")]
public interface IGreetingGrain : IGrainWithStringKey
{
    [Alias("GetAsync")]
    ValueTask<string> GetAsync();

    [Alias("SetAsync")]
    ValueTask<string> SetAsync(string value);

    [Alias("GetAndWait")]
    ValueTask<string> GetAndWait();

    [Alias("DeactivateAsync")]
    Task DeactivateAsync();
}
