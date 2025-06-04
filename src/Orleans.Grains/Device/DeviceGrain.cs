using OrleansGrainInterfaces.Device;

namespace Orleans.Grains.Device;

public class DeviceGrain : Grain, IDeviceGrain
{
    private string _firmwareVersion = "1.0";
    private string _status = "Idle";

    public Task DeactivateAsync()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }

    public Task<string> GetStatus() => Task.FromResult($"{_status} (Firmware: {_firmwareVersion})");

    public async Task<bool> UpdateFirmware(string firmwareVersion)
    {
        _status = "Updating";
        await Task.Delay(2000);
        _firmwareVersion = firmwareVersion;
        _status = "Idle";
        return true;
    }
}
