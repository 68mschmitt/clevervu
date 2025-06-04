namespace OrleansGrainInterfaces.Device;

[Alias("OrleansGrainInterfaces.Device.IDeviceGrain")]
public interface IDeviceGrain : IGrainWithStringKey
{
    [Alias("GetStatus")]
    Task<string> GetStatus();

    [Alias("UpdateFirmware")]
    Task<bool> UpdateFirmware(string firmwareVersion);

    [Alias("DeactivateAsync")]
    Task DeactivateAsync();
}
