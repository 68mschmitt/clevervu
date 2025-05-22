using OrleansGrainInterfaces.Example;

namespace Orleans.Grains.Example;

public class GreetingGrain : Grain, IGreetingGrain
{
    public async override Task OnActivateAsync(CancellationToken token)
    {
        _Greetee = this.GetPrimaryKeyString();
        _FullGreeting = $"Hello, {_Greetee}";

        await base.OnActivateAsync(token);
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

    private string? _Greetee;
    private string _FullGreeting = "";
}
