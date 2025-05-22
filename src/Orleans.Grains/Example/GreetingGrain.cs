using Orleans.Streams;
using OrleansGrainInterfaces.Example;

namespace Orleans.Grains.Example;

public class GreetingGrain : Grain, IAsyncObserver<string>, IGreetingGrain
{
    private StreamSubscriptionHandle<string> _subscriptionHandle;
    public async override Task OnActivateAsync(CancellationToken token)
    {
        _Greetee = this.GetPrimaryKeyString();
        _FullGreeting = $"Hello, {_Greetee}";

        var streamProvider = this.GetStreamProvider("SMS");
        var streamId = StreamId.Create("Greetings", _Greetee);
        var stream = streamProvider.GetStream<string>(streamId);

        _subscriptionHandle = await stream.SubscribeAsync(this);

        await base.OnActivateAsync(token);
    }

    public async override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken token)
    {
        _subscriptionHandle?.UnsubscribeAsync();

        await base.OnDeactivateAsync(reason, token);
    }

    public ValueTask<string> GetAsync()
    {
        return new(_FullGreeting);
    }

    public ValueTask<string> SetAsync(string value)
    {
        _FullGreeting = $"{value}, {_Greetee}";

        return new(_FullGreeting);
    }

    public ValueTask<string> GetAndWait()
    {
        return new(GetWithAWait());
    }

    private async Task<string> GetWithAWait()
    {
        await Task.Delay(1000);
        return _FullGreeting;
    }

    public async Task DeactivateAsync()
    {
        await Task.CompletedTask;
        DeactivateOnIdle();
    }

    public async Task OnNextAsync(string item, StreamSequenceToken? token = null)
    {
        await Task.CompletedTask;
        _FullGreeting = $"{item}, {_Greetee}";
    }

    public Task OnErrorAsync(Exception ex)
    {
        throw new NotImplementedException();
    }

    Task OnCompletedAsync()
    {
        return Task.CompletedTask;
    }

    private string? _Greetee;
    private string _FullGreeting = "";

    public bool IsRewindable => throw new NotImplementedException();

    public string ProviderName => throw new NotImplementedException();

    public StreamId StreamId => throw new NotImplementedException();
}
